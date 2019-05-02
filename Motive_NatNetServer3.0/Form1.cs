//=============================================================================
// Copyright © 2009 NaturalPoint, Inc. All Rights Reserved.
// 
// This software is provided by the copyright holders and contributors "as is" and
// any express or implied warranties, including, but not limited to, the implied
// warranties of merchantability and fitness for a particular purpose are disclaimed.
// In no event shall NaturalPoint, Inc. or contributors be liable for any direct,
// indirect, incidental, special, exemplary, or consequential damages
// (including, but not limited to, procurement of substitute goods or services;
// loss of use, data, or profits; or business interruption) however caused
// and on any theory of liability, whether in contract, strict liability,
// or tort (including negligence or otherwise) arising in any way out of
// the use of this software, even if advised of the possibility of such damage.
//=============================================================================

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;

using NatNetML;
using WampSharp.V2;
using WampSharp.V2.Client;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using WampSharp.V2.Rpc;
using System.Diagnostics;

/*
 *
 * Simple C# .NET sample showing how to use the NatNet managed assembly (NatNETML.dll).
 * 
 * It is designed to illustrate using NatNet.  There are some inefficiencies to keep the
 * code as simple to read as possible.
 * 
 * Sections marked with a [NatNet] are NatNet related and should be implemented in your code.
 * 
 * This sample uses the Microsoft Chart Controls for Microsoft .NET for graphing, which
 * requires the following assemblies:
 *   - System.Windows.Forms.DataVisualization.Design.dll
 *   - System.Windows.Forms.DataVisualization.dll
 * Make sure you have these in your path when building and redistributing.
 * 
 */

namespace NatNetServer
{
    public partial class Form1 : Form
    {
        /// Helper class for discovering NatNet servers.
        private NatNetServerDiscovery m_Discovery = new NatNetServerDiscovery();

        // [NatNet] Our NatNet object
        private NatNetML.NatNetClientML m_NatNet;

        // [NatNet] Our NatNet Frame of Data object
        private NatNetML.FrameOfMocapData m_FrameOfData = new NatNetML.FrameOfMocapData();

        // Time that has passed since the NatNet server transmitted m_FrameOfData.
        private double m_FrameOfDataTransitLatency;

        // [NatNet] Description of the Active Model List from the server (e.g. Motive)
        NatNetML.ServerDescription desc = new NatNetML.ServerDescription();

        // [NatNet] Queue holding our incoming mocap frames the NatNet server (e.g. Motive)
        private Queue<NatNetML.FrameOfMocapData> m_FrontQueue = new Queue<NatNetML.FrameOfMocapData>();
        private Queue<NatNetML.FrameOfMocapData> m_BackQueue = new Queue<NatNetML.FrameOfMocapData>();
        private static object FrontQueueLock = new object();
        private static object BackQueueLock = new object();

        // Records the age of each frame in m_FrameQueue at the time it arrived.
        private Queue<double> m_FrameTransitLatencies = new Queue<double>();

        // data grid
        Hashtable htMarkers = new Hashtable();

        List<RigidBody> mRigidBodies = new List<RigidBody>();
        Hashtable htRigidBodies = new Hashtable();
        Hashtable htRigidBodyMarkers = new Hashtable();

        Hashtable htSkelRBs = new Hashtable();

        List<ForcePlate> mForcePlates = new List<ForcePlate>();
        Hashtable htForcePlates = new Hashtable();

        List<Device> mDevices = new List<Device>();
        Hashtable htDevices = new Hashtable();
        private int mLastRowCount;
        private int minGridHeight;

        // graph
        const int GraphFrames = 10000;
        int m_iLastFrameNumber = 0;
        const int maxSeriesCount = 10;

        // frame timing information
        double m_fLastFrameTimestamp = 0.0f;
        QueryPerfCounter m_FramePeriodTimer = new QueryPerfCounter();
        QueryPerfCounter m_ProcessingTimer = new QueryPerfCounter();
        private double interframeDuration;
        private int droppedFrameIndicator = 0;

        // server information
        double m_ServerFramerate = 1.0f;
        float m_ServerToMillimeters = 1.0f;
        int m_UpAxis = 1;   // 0=x, 1=y, 2=z (Y default)
        int mAnalogSamplesPerMocpaFrame = 0;
        int mDroppedFrames = 0;
        int mLastFrame = 0;
        int mUIBusyCount = 0;
        bool mNeedTrackingListUpdate = false;

        // UI updating
        private delegate void OutputMessageCallback(string strMessage);
        private bool mPaused = false;
        delegate void UpdateUICallback();
        bool mApplicationRunning = true;
        Thread UpdateNatNetDataThread = null;

        // polling
        delegate void PollCallback();
        Thread pollThread;
        bool mPolling = false;

        // recording
        bool mRecording = false;
        TextWriter mWriter;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_Discovery.OnServerDiscovered += delegate (NatNetML.DiscoveredServer server)
            {
                OutputMessage(String.Format(
                    "Discovered server: {0} {1}.{2} at {3} (local interface: {4})",
                    server.ServerDesc.HostApp,
                    server.ServerDesc.HostAppVersion[0],
                    server.ServerDesc.HostAppVersion[1],
                    server.ServerAddress,
                    server.LocalAddress
                ));
            };

            m_Discovery.StartDiscovery();

            // Show available ip addresses of this machine
            String strMachineName = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(strMachineName);
            foreach (IPAddress ip in ipHost.AddressList)
            {
                string strIP = ip.ToString();
                comboBoxLocal.Items.Add(strIP);
            }
            int selected = comboBoxLocal.Items.Add("127.0.0.1");
            comboBoxLocal.SelectedItem = comboBoxLocal.Items[selected];

            // create NatNet client
            // create NatNet client
            int iResult = CreateClient();

            // DataGrid 
            // enable double buffering on DataGridView to optimize cell redraws

            // create and run an Update UI thread
            //UpdateUICallback d = new UpdateUICallback(UpdateUI);
            //UIUpdateThread = new Thread(() =>
            //{
            //    while (mApplicationRunning)
            //    {
            //        try
            //        {
            //            this.Invoke(d);
            //            Thread.Sleep(15);
            //        }
            //        catch (System.Exception ex)
            //        {
            //            OutputMessage(ex.Message);
            //            break;
            //        }
            //    }
            //});
            //UIUpdateThread.Start();

            //// create and run a polling thread
            //PollCallback pd = new PollCallback(PollData);
            //pollThread = new Thread(() =>
            //{
            //    while (mPolling)
            //    {
            //        try
            //        {
            //            this.Invoke(pd);
            //            Thread.Sleep(15);
            //        }
            //        catch (System.Exception ex)
            //        {
            //            OutputMessage(ex.Message);
            //            break;
            //        }
            //    }
            //});

        }

        /// <summary>
        /// Create a new NatNet client, which manages all communication with the NatNet server (e.g. Motive)
        /// </summary>
        /// <param name="iConnectionType">0 = Multicast, 1 = Unicast</param>
        /// <returns></returns>
        private int CreateClient()
        {
            // release any previous instance
            if (m_NatNet != null)
            {
                m_NatNet.Disconnect();
            }

            // [NatNet] create a new NatNet instance
            m_NatNet = new NatNetML.NatNetClientML();

            // [NatNet] set a "Frame Ready" callback function (event handler) handler that will be
            // called by NatNet when NatNet receives a frame of data from the server application
            m_NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);

            /*
            // [NatNet] for testing only - event signature format required by some types of .NET applications (e.g. MatLab)
            m_NatNet.OnFrameReady2 += new FrameReadyEventHandler2(m_NatNet_OnFrameReady2);
            */

            // [NatNet] print version info
            int[] ver = new int[4];
            ver = m_NatNet.NatNetVersion();
            String strVersion = String.Format("NatNet Version : {0}.{1}.{2}.{3}", ver[0], ver[1], ver[2], ver[3]);
            OutputMessage(strVersion);

            return 0;
        }

        #region WAMP
        private bool IsBroadcastingToWAMP = false;
        private string WAMPRouterAdress = "ws://127.0.0.1:8080/ws";
        private const string WAMPRealm = "realm1";
        private string WAMPTopic = "NatNetDataSample";
        private IWampChannel channel = null;
        private IWampSubject WAMPSubject = null;
        private IArgumentsService proxy = null;
        private int counter = 0;
        private int NatNetID = new Random().Next();

        private void CreateWAMPClient()
        {

            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            channel = channelFactory.CreateJsonChannel(WAMPRouterAdress, WAMPRealm);

            counter = 0;
            ConnectToRouter();
        }

        private void ConnectToRouter()
        {
            if (channel != null)
            {
                try
                {
                    DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();
                    channel = channelFactory.CreateJsonChannel(WAMPRouterAdress, WAMPRealm);
                    Task openTask = channel.Open();

                    openTask.Wait(5000);

                    IWampRealmProxy realmProxy = channel.RealmProxy;

                    WAMPSubject = realmProxy.Services.GetSubject(WAMPTopic);

                    OutputMessage("Connected to Router " + WAMPRouterAdress);
                    proxy = channel.RealmProxy.Services.GetCalleeProxy<IArgumentsService>();
                    try
                    {
                        object[] info = new object[2] { "natnet" + NatNetID, "natnet" };
                        proxy.RegisterPublisher("add", info);
                        this.Invoke(new Action<object, bool>(OnFinishConnectProcess), this, true);
                    }
                    catch (Exception ex)
                    {
                        counter++;
                        Thread.Sleep(2000);
                        if (counter <= 30)
                        {
                            ConnectToRouter();
                        }
                        else
                        {
                            OutputMessage("Failed to connect to router: " + WAMPRouterAdress);
                            this.Invoke(new Action<object, bool>(OnFinishConnectProcess), this, false);
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    counter++;
                    Thread.Sleep(2000);
                    if (counter <= 30)
                    {
                        ConnectToRouter();
                    }
                    else
                    {
                        OutputMessage("Failed to connect to router: " + WAMPRouterAdress);
                        this.Invoke(new Action<object, bool>(OnFinishConnectProcess), this, false);
                    }
                }
            }  
        }

        private void DisconnectFromWAMPRouter()
        {
            if(channel != null)
            {
                try
                {
                    channel.Close();
                    channel = null;
                    WAMPSubject = null;
                }
                catch (Exception ex)
                {
                    OutputMessage(ex.Message);
                } 
            }
            IsBroadcastingToWAMP = false;
        }

        private Thread ReconnectToWAMPThread = null;
        
        private void StreamToWAMP(Tuple<string, RigidBodyData, MarkerSetData>[] rbList)
        {
            if (WAMPSubject != null)
            {
                try
                {
                    object[] messageData = new object[rbList.Length];
                    int index = 0;
                    foreach (Tuple<string, RigidBodyData, MarkerSetData> rb in rbList)
                    {
                        object[] container = new object[4];
                        container[0] = rb.Item1; //name
                        container[1] = new object[] { rb.Item2.x, rb.Item2.y, rb.Item2.z }; //location
                        container[2] = new object[] { rb.Item2.qx, rb.Item2.qy, rb.Item2.qz, rb.Item2.qw }; //orientation

                        List<object> markers = new List<object>();
                        for (int i = 0; i < rb.Item3.nMarkers; i++)
                        {
                            if(rb.Item3.Markers[i] != null)
                            {
                                markers.Add(new float[] { rb.Item3.Markers[i].x, rb.Item3.Markers[i].y, rb.Item3.Markers[i].z });
                            }  
                        }
                        container[3] = markers;

                        messageData[index] = container;
                        index++;
                    }

                    WampEvent evt = new WampEvent();
                    evt.Arguments = messageData;

                    WAMPSubject.OnNext(evt);
                }
                catch(WampSessionNotEstablishedException wamp_ex)
                {
                    IsBroadcastingToWAMP = false;
                    OutputMessage("Lost connection to WAMP Router. Attempt to reconnect!");
                    this.Invoke(new Action<object>(OnLostWAMPConnection), this);
                    if (ReconnectToWAMPThread != null && ReconnectToWAMPThread.IsAlive)
                    {
                        ReconnectToWAMPThread.Abort();
                    }
                    ReconnectToWAMPThread = new Thread(new ThreadStart(ConnectToRouter));
                    ReconnectToWAMPThread.Start();
                }
                catch(Exception ex)
                {
                    //skip this frame
                }
            }
        }

        #endregion

        /// <summary>
        /// Connect to a NatNet server (e.g. Motive)
        /// </summary>
        private static ManualResetEvent natnet_pause_event = new ManualResetEvent(true);
        private void Connect()
        {
            // [NatNet] connect to a NatNet server
            int returnCode = 0;
            string strLocalIP = comboBoxLocal.SelectedItem.ToString();
            string strServerIP = textBoxServer.Text;

            NatNetClientML.ConnectParams connectParams = new NatNetClientML.ConnectParams();
            connectParams.ConnectionType = RadioUnicast.Checked ? ConnectionType.Unicast : ConnectionType.Multicast;
            connectParams.ServerAddress = strServerIP;
            connectParams.LocalAddress = strLocalIP;
            returnCode = m_NatNet.Connect(connectParams);
            if (returnCode == 0)
            {
                OutputMessage("Initialization Succeeded.");
            }
            else
            {
                OutputMessage("Error Initializing.");
                //checkBoxConnect.Checked = false;
            }

            returnCode = m_NatNet.GetServerDescription(desc);
            if (returnCode == 0)
            {
                OutputMessage("Connection Succeeded.");
                OutputMessage("   Server App Name: " + desc.HostApp);
                OutputMessage(String.Format("   Server App Version: {0}.{1}.{2}.{3}", desc.HostAppVersion[0], desc.HostAppVersion[1], desc.HostAppVersion[2], desc.HostAppVersion[3]));
                OutputMessage(String.Format("   Server NatNet Version: {0}.{1}.{2}.{3}", desc.NatNetVersion[0], desc.NatNetVersion[1], desc.NatNetVersion[2], desc.NatNetVersion[3]));
                buttonConnectNatNet.Text = "Disconnect";

                // Tracking Tools and Motive report in meters - lets convert to millimeters
                if (desc.HostApp.Contains("TrackingTools") || desc.HostApp.Contains("Motive"))
                    m_ServerToMillimeters = 1000.0f;


                // [NatNet] [optional] Query mocap server for the current camera framerate
                int nBytes = 0;
                byte[] response = new byte[10000];
                int rc;
                rc = m_NatNet.SendMessageAndWait("FrameRate", out response, out nBytes);
                if (rc == 0)
                {
                    try
                    {
                        m_ServerFramerate = BitConverter.ToSingle(response, 0);
                        OutputMessage(String.Format("   Camera Framerate: {0}", m_ServerFramerate));
                    }
                    catch (System.Exception ex)
                    {
                        OutputMessage(ex.Message);
                    }
                }

                // [NatNet] [optional] Query mocap server for the current analog framerate
                rc = m_NatNet.SendMessageAndWait("AnalogSamplesPerMocapFrame", out response, out nBytes);
                if (rc == 0)
                {
                    try
                    {
                        mAnalogSamplesPerMocpaFrame = BitConverter.ToInt32(response, 0);
                        OutputMessage(String.Format("   Analog Samples Per Camera Frame: {0}", mAnalogSamplesPerMocpaFrame));
                    }
                    catch (System.Exception ex)
                    {
                        OutputMessage(ex.Message);
                    }
                }


                // [NatNet] [optional] Query mocap server for the current up axis
                rc = m_NatNet.SendMessageAndWait("UpAxis", out response, out nBytes);
                if (rc == 0)
                {
                    m_UpAxis = BitConverter.ToInt32(response, 0);
                }

                mDroppedFrames = 0;
                lock (FrontQueueLock)
                {
                    m_FrontQueue.Clear();
                }
                lock (BackQueueLock)
                {
                    m_BackQueue.Clear();
                }

                buttonBroadcastToWAMP.Enabled = true;
                IsReceivingData = true;
            }
            else
            {
                OutputMessage("Error Connecting.");
                buttonConnectNatNet.Text = "Connect";
                buttonBroadcastToWAMP.Enabled = false;
            }
        }

        private void Disconnect()
        {
            // [NatNet] disconnect
            // optional : for unicast clients only - notify Motive we are disconnecting
            int nBytes = 0;
            byte[] response = new byte[10000];
            int rc;
            rc = m_NatNet.SendMessageAndWait("Disconnect", out response, out nBytes);
            if (rc == 0)
            {

            }

            // shutdown our client socket
            m_NatNet.Disconnect();
            buttonConnectNatNet.Text = "Connect";
            buttonBroadcastToWAMP.Enabled = false;
        }

        private void OutputMessage(string strMessage)
        {
            if (mPaused)
                return;

            if(!mApplicationRunning)
                return;

            if (this.listView1.InvokeRequired)
            {
                // It's on a different thread, so use Invoke
                OutputMessageCallback d = new OutputMessageCallback(OutputMessage);
                this.Invoke(d, new object[] { strMessage });
            }
            else
            {
                // It's on the same thread, no need for Invoke
                DateTime d = DateTime.Now;
                String strTime = String.Format("{0}:{1}:{2}:{3}", d.Hour, d.Minute, d.Second, d.Millisecond);
                ListViewItem item = new ListViewItem(strTime, 0);
                item.SubItems.Add(strMessage);
                listView1.Items.Add(item);
            }
        }

        private RigidBody FindRB(int id)
        {
            foreach (RigidBody rb in mRigidBodies)
            {
                if (rb.ID == id)
                {
                    return rb;
                }
            }
            return null;
        }

        /// <summary>
        /// Update the spreadsheet.  
        /// Note: This refresh is quite slow and provided here only as a complete example. 
        /// In a production setting this would be optimized.
        /// </summary>

        private void UpdateFrameData()
        {
            List<NatNetML.DataDescriptor> descs = new List<NatNetML.DataDescriptor>();
            bool bSuccess = m_NatNet.GetDataDescriptions(out descs);

            if (bSuccess)
            {
                Dictionary<int, RigidBody> rigidBodies = new Dictionary<int, RigidBody>();

                foreach (NatNetML.DataDescriptor d in descs)
                {
                    // RigidBodies
                    if (d.type == (int)NatNetML.DataDescriptorType.eRigidbodyData)
                    {
                        NatNetML.RigidBody rb = (NatNetML.RigidBody)d;
                        rigidBodies.Add(rb.ID, rb);
                    }
                }
                // update RigidBody data
                Dictionary<string, MarkerSetData> markersets = new Dictionary<string, MarkerSetData>();
                Dictionary<string, int> duplicatedNames = new Dictionary<string, int>();
                List<MarkerSetData> duplicatedNamedMarkerSets = new List<MarkerSetData>();
                for (int i = 0; i < m_FrameOfData.MarkerSets.Length; i++)
                {
                    if(m_FrameOfData.MarkerSets[i] != null)
                    {
                        string name = m_FrameOfData.MarkerSets[i].MarkerSetName;
                        if (markersets.ContainsKey(name))
                        {
                            if (!duplicatedNames.ContainsKey(name))
                            {
                                duplicatedNames.Add(name, -1);
                            }
                            duplicatedNamedMarkerSets.Add(m_FrameOfData.MarkerSets[i]);
                        }
                        else 
                        {
                            markersets.Add(name, m_FrameOfData.MarkerSets[i]);
                        }
                    }
                }
                Tuple<string, RigidBodyData, MarkerSetData>[] dataFrame = new Tuple<string, RigidBodyData, MarkerSetData>[m_FrameOfData.nRigidBodies];
                for (int i = 0; i < m_FrameOfData.nRigidBodies; i++)
                {
                    NatNetML.RigidBodyData rb = m_FrameOfData.RigidBodies[i];
                    
                    string name = "";
                    RigidBody rbDef = null;
                    rigidBodies.TryGetValue(rb.ID, out rbDef);
                    
                    if (rbDef != null)
                    {
                        name = rbDef.Name;
                        //check duplicated list first

                        MarkerSetData markers;

                        int nth;
                        if (duplicatedNames.TryGetValue(name, out nth))
                        {
                            if(nth < 0)
                            {
                                markersets.TryGetValue(name, out markers);
                            }
                            else
                            {
                                markers = duplicatedNamedMarkerSets[nth];
                            }
                            duplicatedNames[name] += 1;
                        }
                        else
                        {
                            markersets.TryGetValue(name, out markers);
                        }
                        
                        dataFrame[i] = new Tuple<string, RigidBodyData, MarkerSetData>(name, rb, markers);
                    }    
                }
                if (IsBroadcastingToWAMP)
                {
                    StreamToWAMP(dataFrame);
                }
            }
        }

        /// <summary>
        /// [NatNet] Request a description of the Active Model List from the server (e.g. Motive) and build up a new spreadsheet  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void GetDataDescription()
        {
            mForcePlates.Clear();
            htForcePlates.Clear();
            mDevices.Clear();
            htDevices.Clear();
            mRigidBodies.Clear();
            htMarkers.Clear();
            htRigidBodies.Clear();
            htRigidBodyMarkers.Clear();
            htSkelRBs.Clear();

            List<NatNetML.DataDescriptor> descs = new List<NatNetML.DataDescriptor>();
            bool bSuccess = m_NatNet.GetDataDescriptions(out descs);
            if (bSuccess)
            {
                //OutputMessage(String.Format("Retrieved {0} Data Descriptions....", descs.Count));
                int iObject = 0;
                foreach (NatNetML.DataDescriptor d in descs)
                {
                    iObject++;

                    // RigidBodies
                    if (d.type == (int)NatNetML.DataDescriptorType.eRigidbodyData)
                    {
                        NatNetML.RigidBody rb = (NatNetML.RigidBody)d;
                        mRigidBodies.Add(rb);
                    }
                }
            }
        }

        private void OnLostWAMPConnection(object sender)
        {
            buttonBroadcastToWAMP.Enabled = false;
        }

        private void OnFinishConnectProcess(object sender, bool successfully)
        {
            if (successfully)
            {
                if (UpdateNatNetDataThread == null)
                {
                    UpdateNatNetDataThread = new Thread(() =>
                    {
                        while (mApplicationRunning)
                        {
                            natnet_pause_event.WaitOne();
                            UpdateNatNetData();
                            Thread.Sleep(15);
                        }
                    });
                    UpdateNatNetDataThread.Priority = ThreadPriority.AboveNormal;
                    UpdateNatNetDataThread.Start();
                }
                else
                {
                    natnet_pause_event.Set();
                }
                IsBroadcastingToWAMP = true;
                buttonConnectNatNet.Enabled = false;
                buttonBroadcastToWAMP.Text = "Stop Broadcasting To WAMP";
            }
            else
            {
                IsBroadcastingToWAMP = false;
                buttonConnectNatNet.Enabled = true;
                buttonBroadcastToWAMP.Text = "Broadcast To WAMP";
                if (UpdateNatNetDataThread != null)
                {
                    natnet_pause_event.Reset();
                }
            }
            buttonBroadcastToWAMP.Enabled = true;
            counter = 0;
        }

        private Thread ConnectToWAMPRouterThread = null;

        private void buttonBroadcastToWAMP_Click(object sender, EventArgs e)
        {
            //Connect to WAMP Router
            if(IsBroadcastingToWAMP)
            {
                if (UpdateNatNetDataThread != null)
                {
                    natnet_pause_event.Reset();
                }
                DisconnectFromWAMPRouter();
                buttonConnectNatNet.Enabled = true;
                buttonBroadcastToWAMP.Text = "Broadcast To WAMP";
            }
            else
            {
                buttonConnectNatNet.Enabled = false;
                buttonBroadcastToWAMP.Enabled = false;
                WAMPRouterAdress = "ws://" + textBoxRouter.Text + ":" + textBoxWAMPPort.Text + "/ws";
                if(ConnectToWAMPRouterThread != null && ConnectToWAMPRouterThread.IsAlive)
                {
                    ConnectToWAMPRouterThread.Abort();
                }
                ConnectToWAMPRouterThread = new Thread(new ThreadStart(CreateWAMPClient));
                ConnectToWAMPRouterThread.Start();
            }

        }

        void ProcessFrameOfData(ref NatNetML.FrameOfMocapData data)
        {
            // detect and reported any 'reported' frame drop (as reported by server)
            if (m_fLastFrameTimestamp != 0.0f)
            {
                double framePeriod = 1.0f / m_ServerFramerate;
                double thisPeriod = data.fTimestamp - m_fLastFrameTimestamp;
                double fudgeFactor = 0.002f; // 2 ms
                if ((thisPeriod - framePeriod) > fudgeFactor)
                {
                    //OutputMessage("Frame Drop: ( ThisTS: " + data.fTimestamp.ToString("F3") + "  LastTS: " + m_fLastFrameTimestamp.ToString("F3") + " )");
                    mDroppedFrames++;
                    droppedFrameIndicator = 10;
                }
                else
                {
                    droppedFrameIndicator = 0;
                }
            }

            // check and report frame drop (frame id based)
            if (mLastFrame != 0)
            {
                if ((data.iFrame - mLastFrame) != 1)
                {
                    //OutputMessage("Frame Drop: ( ThisFrame: " + data.iFrame.ToString() + "  LastFrame: " + mLastFrame.ToString() + " )");
                    //mDroppedFrames++;
                }
            }

            if (data.bTrackingModelsChanged)
                mNeedTrackingListUpdate = true;

            FrameOfMocapData deepCopy = new FrameOfMocapData(data);
            // [NatNet] Add the incoming frame of mocap data to our frame queue,  
            // Note: the frame queue is a shared resource with the UI thread, so lock it while writing
            lock (BackQueueLock)
            {
                m_BackQueue.Enqueue(deepCopy);
                // limit background queue size to 10 frames
                while (m_BackQueue.Count > 10)
                {
                    m_BackQueue.Dequeue();
                }
            }

            // Update the shared UI queue, only if the UI thread is not updating (we don't want to wait here as we're in the data update thread)
            bool lockAcquired = false;
            try
            {
                Monitor.TryEnter(FrontQueueLock, ref lockAcquired);
                if (lockAcquired)
                {
                    // [optional] clear the frame queue before adding a new frame (UI only wants most recent frame)
                    m_FrontQueue.Clear();
                    m_FrontQueue.Enqueue(deepCopy);

                    m_FrameTransitLatencies.Clear();
                    m_FrameTransitLatencies.Enqueue(m_NatNet.SecondsSinceHostTimestamp(data.TransmitTimestamp));
                }
                else
                {
                    mUIBusyCount++;
                }
            }
            finally
            {
                if (lockAcquired)
                    Monitor.Exit(FrontQueueLock);
            }

            // recording : write packet to data file
            if (mRecording)
            {
                WriteFrame(deepCopy);
            }

            mLastFrame = data.iFrame;
            m_fLastFrameTimestamp = data.fTimestamp;
        }

        /// <summary>
        /// [NatNet] m_NatNet_OnFrameReady will be called when a frame of Mocap
        /// data has is received from the server application.
        ///
        /// Note: This callback is on the network service thread, so it is
        /// important to return from this function quickly as possible 
        /// to prevent incoming frames of data from buffering up on the
        /// network socket.
        ///
        /// Note: "data" is a reference structure to the current frame of data.
        /// NatNet re-uses this same instance for each incoming frame, so it should
        /// not be kept (the values contained in "data" will become replaced after
        /// this callback function has exited).
        /// </summary>
        /// <param name="data">The actual frame of mocap data</param>
        /// <param name="client">The NatNet client instance</param>
        void m_NatNet_OnFrameReady(NatNetML.FrameOfMocapData data, NatNetML.NatNetClientML client)
        {
            // measure time between frame arrival (inter frame)
            m_FramePeriodTimer.Stop();
            interframeDuration = m_FramePeriodTimer.Duration();

            // measure processing time (intra frame)
            m_ProcessingTimer.Start();

            // process data
            // NOTE!  do as little as possible here as we're on the data servicing thread
            ProcessFrameOfData(ref data);

            // report if we are taking longer than a mocap frame time
            // which eventually will back up the network receive buffer and result in frame drop
            m_ProcessingTimer.Stop();
            double appProcessingTimeMSecs = m_ProcessingTimer.Duration();
            double mocapFramePeriodMSecs = (1.0f / m_ServerFramerate) * 1000.0f;
            //if (appProcessingTimeMSecs > mocapFramePeriodMSecs)
            //{
            //    OutputMessage("Warning : Frame handler taking longer than frame period: " + appProcessingTimeMSecs.ToString("F2"));
            //}

            m_FramePeriodTimer.Start();

        }

        // [NatNet] [optional] alternate function signatured frame ready callback handler for .NET applications/hosts
        // that don't support the m_NatNet_OnFrameReady defined above (e.g. MATLAB)
        void m_NatNet_OnFrameReady2(object sender, NatNetEventArgs e)
        {
            m_NatNet_OnFrameReady(e.data, e.client);
        }

        private void PollData()
        {
            FrameOfMocapData data = m_NatNet.GetLastFrameOfData();
            ProcessFrameOfData(ref data);
        }

        private void SetDataPolling(bool poll)
        {
            if (poll)
            {
                // disable event based data handling
                m_NatNet.OnFrameReady -= m_NatNet_OnFrameReady;

                // enable polling 
                mPolling = true;
                pollThread.Start();
            }
            else
            {
                // disable polling
                mPolling = false;

                // enable event based data handling
                m_NatNet.OnFrameReady += new NatNetML.FrameReadyEventHandler(m_NatNet_OnFrameReady);
            }
        }

        private void GetLastFrameOfData()
        {
            FrameOfMocapData data = m_NatNet.GetLastFrameOfData();
            ProcessFrameOfData(ref data);
        }

        private void GetLastFrameOfDataButton_Click(object sender, EventArgs e)
        {
            // [NatNet] GetLastFrameOfData can be used to poll for the most recent avail frame of mocap data.
            // This mechanism is slower than the event handler mechanism, and in general is not recommended,
            // since it must wait for a frame to become available and apply a lock to that frame while it copies
            // the data to the returned value.

            // get a copy of the most recent frame of data
            // returns null if not available or cannot obtain a lock on it within a specified timeout
            FrameOfMocapData data = m_NatNet.GetLastFrameOfData();
            if (data != null)
            {
                // do something with the data
                String frameInfo = String.Format("FrameID : {0}", data.iFrame);
                OutputMessage(frameInfo);
            }
        }

        private void WriteFrame(FrameOfMocapData data)
        {
            String str =  "";

            str += data.fTimestamp.ToString("F3") + "\t";

            // 'all' markerset data
            for (int i = 0; i < m_FrameOfData.nMarkerSets; i++)
            {
                NatNetML.MarkerSetData ms = m_FrameOfData.MarkerSets[i];
                if(ms.MarkerSetName == "all")
                {
                   for (int j = 0; j < ms.nMarkers; j++)
                    {
                       str += ms.Markers[j].x.ToString("F3") + "\t";
                       str += ms.Markers[j].y.ToString("F3") + "\t";
                       str += ms.Markers[j].z.ToString("F3") + "\t";
                    }
                }
            }

            // force plates
            // just write first subframe from each channel (fx[0], fy[0], fz[0], mx[0], my[0], mz[0])
            for (int i = 0; i < m_FrameOfData.nForcePlates; i++)
            {
                NatNetML.ForcePlateData fp = m_FrameOfData.ForcePlates[i];
                for(int iChannel=0; iChannel < fp.nChannels; iChannel++)
                {
                    if(fp.ChannelData[iChannel].nFrames == 0)
                    {
                        str += 0.0f;    // empty frame
                    }
                    else
                    {
                        str += fp.ChannelData[iChannel].Values[0] + "\t";
                    }
                }
            }

            mWriter.WriteLine(str);
        }

        private void Reconnect()
        {
            buttonBroadcastToWAMP.Enabled = false;
            buttonConnectNatNet.Enabled = false;
            Disconnect();
            Thread.Sleep(15);
            Connect();
        }

        private const int CONNECTION_TIMEOUT = 5;
        private bool IsReceivingData = false;
        private Stopwatch timeoutWatch = new Stopwatch();

        private void UpdateNatNetData()
        {
            // the frame queue is a shared resource with the FrameOfMocap delivery thread, so lock it while reading
            // note this can block the frame delivery thread.  In a production application frame queue management would be optimized.
            bool lockAcquired = false;
            try
            {
                Monitor.TryEnter(FrontQueueLock, ref lockAcquired);
                if (lockAcquired)
                {
                    if (m_FrontQueue.Count > 0)
                    {
                        while (m_FrontQueue.Count > 0)
                            m_FrameOfData = m_FrontQueue.Dequeue();
                        while (m_FrameTransitLatencies.Count > 0)
                            m_FrameOfDataTransitLatency = m_FrameTransitLatencies.Dequeue();

                        Monitor.Exit(FrontQueueLock);
                        lockAcquired = false;

                        // update the data grid
                        UpdateFrameData();
                    }
                    if (!IsReceivingData && !timeoutWatch.IsRunning)
                    {
                        timeoutWatch.Restart();
                    }
                    else
                    {
                        IsReceivingData = false;
                        timeoutWatch.Stop();
                    }
                    if (timeoutWatch.IsRunning)
                    {
                        if (timeoutWatch.Elapsed.Seconds > CONNECTION_TIMEOUT) //lose connection to Motive, attempt to reconnect
                        {
                            timeoutWatch.Stop();
                            OutputMessage("Lost connection to Motive, attempt to reconnect!");
                            UpdateUICallback d = new UpdateUICallback(Reconnect);
                            this.Invoke(d);
                            Thread.Sleep(2000);
                        }
                    }
                }
            }
            finally
            {
                if (lockAcquired)
                {
                    Monitor.Exit(FrontQueueLock);
                }
            }
        }

        public int LowWord(int number)
        {
            return number & 0xFFFF;
        }

        public int HighWord(int number)
        {
            return ((number >> 16) & 0xFFFF);
        }

        double RadiansToDegrees(double dRads)
        {
            return dRads * (180.0f / Math.PI);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            mApplicationRunning = false;

            if(UpdateNatNetDataThread != null && UpdateNatNetDataThread.IsAlive)
                UpdateNatNetDataThread.Abort();
            DisconnectFromWAMPRouter();
            if (ConnectToWAMPRouterThread != null && ConnectToWAMPRouterThread.IsAlive)
                ConnectToWAMPRouterThread.Abort();
            if (ReconnectToWAMPThread != null && ReconnectToWAMPThread.IsAlive)
                ReconnectToWAMPThread.Abort();
            m_NatNet.Disconnect();
        }

        private void RadioMulticast_CheckedChanged(object sender, EventArgs e)
        {
            bool bNeedReconnect = buttonConnectNatNet.Text.Equals("Disconnect");
            if (bNeedReconnect)
                Connect();
        }

        private void RadioUnicast_CheckedChanged(object sender, EventArgs e)
        {
            bool bNeedReconnect = buttonConnectNatNet.Text.Equals("Disconnect");
            if (bNeedReconnect)
                Connect();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void menuClear_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void menuPause_Click(object sender, EventArgs e)
        {
            mPaused = menuPause.Checked;
        }

        private void GetTakeRangeButton_Click(object sender, EventArgs e)
        {
            int nBytes = 0;
            byte[] response = new byte[10000];
            int rc;
            rc = m_NatNet.SendMessageAndWait("CurrentTakeLength", out response, out nBytes);
            if (rc == 0)
            {
                try
                {
                    int takeLength = BitConverter.ToInt32(response, 0);
                    OutputMessage(String.Format("Current Take Length: {0}", takeLength));
                }
                catch (System.Exception ex)
                {
                    OutputMessage(ex.Message);
                }
            }
        }

        private void GetModeButton_Click(object sender, EventArgs e)
        {
            int nBytes = 0;
            byte[] response = new byte[10000];
            int rc;
            rc = m_NatNet.SendMessageAndWait("CurrentMode", out response, out nBytes);
            if (rc == 0)
            {
                try
                {
                    String strMode = "";
                    int mode = BitConverter.ToInt32(response, 0);
                    if (mode == 0)
                        strMode = String.Format("Mode : Live");
                    else if (mode == 1)
                        strMode = String.Format("Mode : Recording");
                    else if (mode == 2)
                        strMode = String.Format("Mode : Edit");
                    OutputMessage(strMode);
                }
                catch (System.Exception ex)
                {
                    OutputMessage(ex.Message);
                }
            }
        }

        private void PollCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBoxServer_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonConnectNatNet_Click(object sender, EventArgs e)
        {
            if (buttonConnectNatNet.Text.Equals("Connect"))
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        private void bindingSource2_CurrentChanged(object sender, EventArgs e)
        {

        }
    }

    public interface IArgumentsService
    {
        [WampProcedure("registerPublisher")]
        void RegisterPublisher(string evt, object msg);
    }

    // Wrapper class for the windows high performance timer QueryPerfCounter
    // ( adapted from MSDN https://msdn.microsoft.com/en-us/library/ff650674.aspx )
    public class QueryPerfCounter
    {
        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long start;
        private long stop;
        private long frequency;
        Decimal multiplier = new Decimal(1.0e9);

        public QueryPerfCounter()
        {
            if (QueryPerformanceFrequency(out frequency) == false)
            {
                // Frequency not supported
                throw new Win32Exception();
            }
        }

        public void Start()
        {
            QueryPerformanceCounter(out start);
        }

        public void Stop()
        {
            QueryPerformanceCounter(out stop);
        }

        // return elapsed time between start and stop, in milliseconds.
        public double Duration()
        {
            double val = ((double)(stop - start) * (double)multiplier) / (double)frequency;
            val = val / 1000000.0f;   // convert to ms
            return val;
        }
    }
}