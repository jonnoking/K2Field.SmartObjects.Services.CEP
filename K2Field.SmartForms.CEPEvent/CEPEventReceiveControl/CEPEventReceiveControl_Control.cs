using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//SourceCode.Forms.Controls.Web.SDK.dll, located in the GAC of the smartforms server or in the bin folder of the K2 Designer web site
using SourceCode.Forms.Controls.Web.SDK;
using SourceCode.Forms.Controls.Web.SDK.Attributes;

//adds the client-side .js file as a web resource
[assembly: WebResource("K2Field.SmartForms.CEPEvent.CEPEventReceiveControl.CEPEventReceiveControl_Script.js", "text/javascript", PerformSubstitution = true)]
//adds the client-side style sheet as a web resource
[assembly: WebResource("K2Field.SmartForms.CEPEvent.CEPEventReceiveControl.CEPEventReceiveControl_Stylesheet.css", "text/css", PerformSubstitution = true)]

namespace K2Field.SmartForms.CEPEvent.CEPEventReceiveControl
{
    //specifies the location of the embedded definition xml file for the control
    [ControlTypeDefinition("K2Field.SmartForms.CEPEvent.CEPEventReceiveControl.CEPEventReceiveControl_Definition.xml")]
    //specifies the location of the embedded client-side javascript file for the control
    [ClientScript("K2Field.SmartForms.CEPEvent.CEPEventReceiveControl.CEPEventReceiveControl_Script.js")]
    //specifies the location of the embedded client-side stylesheet for the control
    [ClientCss("K2Field.SmartForms.CEPEvent.CEPEventReceiveControl.CEPEventReceiveControl_Stylesheet.css")]
    //specifies the location of the client-side resource file for the control. You will need to add a resource file to the project properties
    //[ClientResources("K2Field.SmartForms.CEPEvent.Resources.[ResrouceFileName]")]
    public class Control : BaseControl
    {
        #region Control Properties
        //to be able to use the control's properties in code-behind, define public properties 
        //with the same names as the properties defined in the Definition.xml file's <Properties> section
        //create get/set methods and return the property of the same name but to lower case

        //in this example, we are exposing the <Prop ID="ControlText"> property from the definition.xml file to the code-behind
        //public string ControlText
        //{
        //    get
        //    {
        //        return this.Attributes["controltext"];
        //    }
        //    set
        //    {
        //        this.Attributes["controltext"] = value;
        //    }
        //}

        //IsVisible property
        public bool IsVisible
        {
            get
            {
                return this.GetOption<bool>("isvisible", true);
            }
            set
            {
                this.SetOption<bool>("isvisible", value, true);
            }
        }

        //IsEnabled property
        public bool IsEnabled
        {
            get
            {
                return this.GetOption<bool>("isenabled", true);
            }
            set
            {
                this.SetOption<bool>("isenabled", value, true);
            }
        }

        //Value property. 
        //"Value" is the value set with the standard getValue/getValue js methods. You can override these methods to set a different property
        public string Value
        {
            get { return this.Attributes["value"]; }
            set { this.Attributes["value"] = value; }
        }

        //public bool OutputDebugInfo
        //{
        //    get
        //    {
        //        return this.GetOption<bool>("outputdebuginfo", true);
        //    }
        //    set
        //    {
        //        this.SetOption<bool>("outputdebuginfo", value, true);
        //    }
        //}

        public string HubUrl
        {
            get
            {
                return this.Attributes["data-huburl"];
            }
            set
            {
                this.Attributes["data-huburl"] = value;
            }
        }

        public string EventFilter
        {
            get
            {
                return this.Attributes["data-eventfilter"];
            }
            set
            {
                this.Attributes["data-eventfilter"] = value;
            }
        }


        public string EId
        {
            get
            {
                return this.Attributes["data-eid"];
            }
            set
            {
                this.Attributes["data-eid"] = value;
            }
        }

        public string Origin
        {
            get
            {
                return this.Attributes["data-origin"];
            }
            set
            {
                this.Attributes["data-origin"] = value;
            }
        }

        public string EventDisplayName
        {
            get
            {
                return this.Attributes["data-eventdisplayname"];
            }
            set
            {
                this.Attributes["data-eventdisplayname"] = value;
            }
        }
        public string EventType
        {
            get
            {
                return this.Attributes["data-eventtype"];
            }
            set
            {
                this.Attributes["data-eventtype"] = value;
            }
        }
        public string EventSource
        {
            get
            {
                return this.Attributes["data-eventsource"];
            }
            set
            {
                this.Attributes["data-eventsource"] = value;
            }
        }
        public string Action
        {
            get
            {
                return this.Attributes["data-action"];
            }
            set
            {
                this.Attributes["data-action"] = value;
            }
        }
        public string ProcessName
        {
            get
            {
                return this.Attributes["data-processname"];
            }
            set
            {
                this.Attributes["data-processname"] = value;
            }
        }
        public string ProcessId
        {
            get
            {
                return this.Attributes["data-processid"];
            }
            set
            {
                this.Attributes["data-processid"] = value;
            }
        }
        public string EventData
        {
            get
            {
                return this.Attributes["data-eventdata"];
            }
            set
            {
                this.Attributes["data-eventdata"] = value;
            }
        }
        public string EventDate
        {
            get
            {
                return this.Attributes["data-eventdate"];
            }
            set
            {
                this.Attributes["data-eventdate"] = value.ToString();
            }
        }
        public string EventId
        {
            get
            {
                return this.Attributes["data-eventid"];
            }
            set
            {
                this.Attributes["data-eventid"] = value;
            }
        }
        public string Status
        {
            get
            {
                return this.Attributes["data-status"];
            }
            set
            {
                this.Attributes["data-status"] = value;
            }
        }
        public string EventListenerId
        {
            get
            {
                return this.Attributes["data-eventlistenerid"];
            }
            set
            {
                this.Attributes["data-eventlistenerid"] = value;
            }
        }

        #region IDs
        public string ControlID
        {
            get
            {
                return base.ID;
            }
            set
            {
                base.ID = value;
            }
        }

        public override string ClientID
        {
            get
            {
                return base.ID;
            }
        }

        public override string UniqueID
        {
            get
            {
                return base.ID;
            }
        }
        #endregion

        #endregion

        #region Contructor
        public Control()
            : base("div")  //TODO: if needed, inherit from a HTML type like div or input
        {

        }
        #endregion

        #region Control Methods
        protected override void CreateChildControls()
        {
            base.EnsureChildControls();

            //TODO: if necessary, create child controls for the control.

            //Perform state-specific operations
            switch (base.State)
            {
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Designtime:
                    //assign a temp unique Id for the control
                    this.ID = Guid.NewGuid().ToString();
                    break;
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Preview:
                    //do any Preview-time manipulation here
                    break;
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Runtime:
                    //do any runtime manipulation here
                    this.Attributes.Add("enabled", this.IsEnabled.ToString());
                    this.Attributes.Add("visible", this.IsVisible.ToString());
                    this.Attributes.Add("style", "display:none;");
                    break;
            }
            this.Controls.Add(AddLabelControlWithControlProperties());

            //if outputting the debug info for the control, add the literal control to the controls collection
            //if (OutputDebugInfo)
            //{
            //    this.Controls.Add(AddLabelControlWithControlProperties());
            //}

            // Call base implementation last
            base.CreateChildControls();
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            //TODO: if needed, implement a method to render contents
        }
        #endregion

        /// <summary>
        /// this helper method outputs a label control with various properties for the Smartforms control
        /// it is intended for development and debugging purposes so that you can output the various properties of your custom control
        /// Feel free to add code and properties to the output element
        /// </summary>
        private LiteralControl AddLabelControlWithControlProperties()
        {
            Label propertiesLabel = new Label();
            string id = string.Empty;
            //populate the value of the label control with the properties of your custom control
            switch (base.State)
            {
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Designtime:
                    this.ID = Guid.NewGuid().ToString() + "_propertiesLabel";
                    propertiesLabel.Text = "[ CEP Event Receive ]";
                    break;
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Preview:
                    propertiesLabel.Text = "[ CEP Event Receive ]";
                    break;
                case SourceCode.Forms.Controls.Web.Shared.ControlState.Runtime:
                    id = this.ID + "_propertiesLabel";
                    //propertiesLabel.Text = "(" + this.GetType().FullName + " - Runtime) " +
                    //    System.Environment.NewLine + "Control Text: " + this.ControlText +
                    //    System.Environment.NewLine + "Control Value: " + this.Value +
                    //    System.Environment.NewLine + "Control Id: " + this.ID +
                    //    System.Environment.NewLine + "Enabled: " + this.IsEnabled +
                    //    System.Environment.NewLine + "Visible: " + this.IsVisible;
                    break;
            }

            //after defining the control, get the control's HTML so we can inject it into the Div placeholder
            string controlHTML = "";

            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                propertiesLabel.RenderControl(writer);
                controlHTML = writer.InnerWriter.ToString();
            }

            LiteralControl controlProperties = new LiteralControl("<div>" + controlHTML + "</div>");

            return controlProperties;
        }
    }
}
