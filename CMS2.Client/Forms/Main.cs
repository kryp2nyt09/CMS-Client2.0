using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CMS2.BusinessLogic;
using CMS2.Common.Constants;
using CMS2.Common.Enums;
using CMS2.Entities;
using CMS2.Entities.Models;
using System.Net.Mail;
using System.Data;
using Telerik.WinControls.UI;
using Microsoft.AspNet.Identity;
using CMS2.Common.Identity;
using CMS2.Client.ViewModels;
using CMS2.DataAccess;
using System.Security.Principal;
using CMS2.Common;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CMS2.Client.ReportModels;
using System.Drawing.Printing;
using CMS2.Client.SyncHelper;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using CMS2.Client.Properties;
using Tools = CMS2.Common.Utilities;
using CMS2.Client.Forms;
using CMS2.Client.Forms.TrackingReports;
using System.IO;
using Telerik.WinControls.Data;
using Telerik.WinControls.Export;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Pdf.Documents.Media;
using CMS2.Client.Forms.TrackingReportsView;
using Telerik.Reporting.Processing;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMS2.Client
{
    public partial class Main : Telerik.WinControls.UI.RadForm
    {

        #region Properties

        #region Main


        private UserManager<IdentityUser, Guid> _userManager;
        private AreaBL areaService;
        private BranchSatOfficeBL bsoService;
        private GatewaySatOfficeBL gatewaySatService;
        private BranchCorpOfficeBL bcoService;
        private RevenueUnitBL revenueUnitService;
        private UserStore userService;
        #endregion

        #region Booking

        Booking booking;
        Entities.Client shipper;
        Entities.Client consignee;
        private AutoCompleteStringCollection shipperLastNames;
        private AutoCompleteStringCollection shipperFirstNames;
        private AutoCompleteStringCollection shipperCompany;
        private AutoCompleteStringCollection clientConsigneeLastNames;
        private AutoCompleteStringCollection clientConsigneeFirstNames;
        private AutoCompleteStringCollection consigneeCompany;
        private AutoCompleteStringCollection shipperBco;
        private AutoCompleteStringCollection shipperCity;
        private AutoCompleteStringCollection consgineeBco;
        private AutoCompleteStringCollection consgineeCity;
        private AutoCompleteStringCollection assignedTo;
        private BindingSource bsBookingStatus;
        private BindingSource bsBookingRemark;
        private BindingSource bsAreas;
        private BindingSource bsOriginBco;
        private BindingSource bsDestinationBco;


        private BookingStatusBL bookingStatusService;
        private BookingRemarkBL bookingRemarkService;
        private BookingBL bookingService;
        private ClientBL clientService;
        private CityBL cityService;
        private CompanyBL companyService;

        private List<BookingStatus> bookingStatus;
        private List<BookingRemark> bookingRemarks;
        private List<BranchCorpOffice> branchCorpOffices;
        private List<RevenueUnit> areas;
        private List<Entities.Client> clients;
        private List<RevenueUnit> revenueUnits;
        private List<City> cities;
        private List<Company> companies;

        #endregion

        #region Acceptance

        private ShipmentModel shipment;
        private PackageDimensionModel packageDimensionModel;
        private BindingSource bsCommodityType;
        private BindingSource bsCommodity;
        private BindingSource bsServiceType;
        private BindingSource bsServiceMode;
        private BindingSource bsPaymentMode;
        private BindingSource bsCrating;
        private BindingSource bsPackaging;
        private BindingSource bsGoodsDescription;
        private BindingSource bsShipMode;
        private BindingSource bsCollectedBy;
        private BindingSource bsRevenueUnitType;
        private AutoCompleteStringCollection commodityTypeCollection;
        private AutoCompleteStringCollection commodityCollection;
        private AutoCompleteStringCollection serviceTypeCollection;
        private AutoCompleteStringCollection serviceModeCollection;
        private AutoCompleteStringCollection shipModeCollection;
        private AutoCompleteStringCollection goodsDescCollection;
        private AutoCompleteStringCollection paymentModeCollection;
        private AutoCompleteStringCollection AirwayBill;

        private CommodityTypeBL commodityTypeService;
        private CommodityBL commodityService;
        private ServiceTypeBL serviceTypeService;
        private ServiceModeBL serviceModeService;
        private PaymentModeBL paymentModeService;
        private ShipmentBL shipmentService;
        private ShipmentBasicFeeBL shipmentBasicFeeService;
        private CratingBL cratingService;
        private PackagingBL packagingService;
        private GoodsDescriptionBL goodsDescriptionService;
        private ShipModeBL shipModeService;
        private RateMatrixBL rateMatrixService;
        private PaymentTermBL paymentTermService;


        private CommodityType commodityType;
        private List<CommodityType> commodityTypes;
        private List<Commodity> commodities;
        private List<ServiceType> serviceTypes;
        private List<ServiceMode> serviceModes;
        private List<PaymentMode> paymentModes;
        private List<ShipmentBasicFee> shipmentBasicFees;
        private List<Crating> cratings;
        private List<Packaging> packagings;
        private List<GoodsDescription> goodsDescriptions;
        private List<ShipMode> shipModes;
        private List<PaymentTerm> paymentTerms;


        public string LogPath;

        public bool isNewShipment { get; set; }

        public ShipmentModel shipmentModel { get; set; }

        #endregion

        #region Payment

        private StatementOfAccountPayment soaPayment;
        private Payment payment;
        private StatementOfAccount soa;
        private BindingSource bsPaymentType;
        public PaymentDetailsViewModel NewPayment;
        //private ShipmentBL shipmentService;
        private StatementOfAccountBL soaService;
        private PaymentBL paymentService;
        private PaymentTypeBL paymentTypeService;

        #endregion

        #region PaymentSummary
        private AutoCompleteStringCollection autoComprevenueUnitName;
        private AutoCompleteStringCollection autoComp_empName;
        private AutoCompleteStringCollection autoComp_revenueUnitType;

        Employee employee;
        Entities.PaymentSummary paymentSummary;
        PaymentSummaryStatus paymentSummaryStatus;

        private List<Employee> employees;
        private List<Payment> paymentPrepaid;
        private List<Payment> paymentFreightCollect;
        private List<Payment> paymentCorpAcctConsignee;
        private List<RevenueUnit> paymentSummary_revenueUnits;
        private List<Employee> paymentSummary_employee;
        private List<Employee> paymentSummary_remittedBy;
        private List<RevenueUnitType> paymentSummary_revenueUnitType;

        public List<PaymentSummaryModel> listPaymentSummary = new List<PaymentSummaryModel>();
        public List<PaymentSummaryDetails> listpaymentSummaryDetails = new List<PaymentSummaryDetails>();
        public List<PaymentSummary_MainDetailsModel> listMainDetails = new List<PaymentSummary_MainDetailsModel>();
        public List<PaymentSummaryModel> passListofPaymentSummary = new List<PaymentSummaryModel>();

        private EmployeeBL employeeService;
        private RevenueUnitTypeBL revenueUnitTypeService;
        private RevenueUnitBL revenueUnitservice;
        private PaymentSummaryStatusBL paymentSummaryStatusService;
        private PaymentSummaryBL paymentSummaryService;

        public int ctrPrepaid = 0;
        public int ctrfreight = 0;
        public int ctrcorpAcct = 0;
        public decimal totalCashReceived = 0;
        public decimal totalCheckReceived = 0;
        public decimal totalAmountReceived = 0;
        public decimal difference = 0;
        public decimal totalCollection = 0;
        private Point? _Previous = null;
        Image signatureImage;


        #endregion

        #endregion

        #region Constructors

        public Main()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        #region Main
        private void Main_Shown(object sender, EventArgs e)
        {
            Login();


            try
            {
                if (AppUser.User.UserName == "admin")
                {
                    btnSettings.Enabled = true;
                }
                else
                {
                    btnSettings.Enabled = false;
                }
            }
            catch (Exception)
            {

            }
        }
        private void Main_Load(object sender, EventArgs e)
        {

            GlobalVars.UnitOfWork = new CmsUoW();
            areaService = new AreaBL(GlobalVars.UnitOfWork);
            bsoService = new BranchSatOfficeBL(GlobalVars.UnitOfWork);
            gatewaySatService = new GatewaySatOfficeBL(GlobalVars.UnitOfWork);
            bcoService = new BranchCorpOfficeBL(GlobalVars.UnitOfWork);
            revenueUnitService = new RevenueUnitBL(GlobalVars.UnitOfWork);

            GlobalVars.DeviceRevenueUnitId = Guid.Parse(ConfigurationManager.AppSettings["RUId"]);
            GlobalVars.DeviceBcoId = Guid.Parse(ConfigurationManager.AppSettings["BcoId"]);
            GlobalVars.UnitOfWork = new CmsUoW();

            LoadInit();

            BookingResetAll();
            PopulateGrid();
            AddDailyBooking();

            AcceptanceLoadInit();
            PaymentSummaryLoadInit();
        }
        private void radPageView1_SelectedPageChanged(object sender, EventArgs e)
        {
            switch (pageViewMain.SelectedPage.Text)
            {
                case "Booking":

                    bsBookingStatus.ResetBindings(false);
                    bsBookingStatus.ResetBindings(false);
                    bsBookingRemark.ResetBindings(false);
                    bsAreas.ResetBindings(false);
                    bsOriginBco.ResetBindings(false);
                    bsDestinationBco.ResetBindings(false);

                    var _areas = areas.Where(x => x.City.BranchCorpOfficeId == GlobalVars.DeviceBcoId).ToList();
                    lstAssignedTo.DataSource = _areas;
                    lstAssignedTo.DisplayMember = "RevenueUnitName";
                    lstAssignedTo.ValueMember = "RevenueUnitId";

                    BookingResetAll();
                    PopulateGrid();

                    break;
                case "Acceptance":
                    if (isNewShipment)
                    {
                        AcceptanceResetAll();
                        ClearSummaryData();
                        AcceptanceLoadData();
                    }
                    else
                    {
                        DisableForm();

                    }

                    break;
                case "Payment":
                    bsPaymentType = new BindingSource();
                    shipmentService = new ShipmentBL(GlobalVars.UnitOfWork);
                    soaService = new StatementOfAccountBL(GlobalVars.UnitOfWork);
                    paymentService = new PaymentBL(GlobalVars.UnitOfWork);
                    paymentTypeService = new PaymentTypeBL(GlobalVars.UnitOfWork);

                    soaPayment = null;
                    payment = null;
                    soa = null;

                    datePaymentDate.Value = DateTime.Now;
                    dateCheckDate.Value = DateTime.Now;

                    bsPaymentType.DataSource = paymentTypeService.FilterActive().OrderBy(x => x.PaymentTypeName).ToList();
                    lstPaymentType.DataSource = bsPaymentType;
                    lstPaymentType.DisplayMember = "PaymentTypeName";
                    lstPaymentType.ValueMember = "PaymentTypeId";

                    if (NewPayment != null)
                    {
                        txtAwb.Text = NewPayment.AwbSoa;
                        txtAmountDue.Text = NewPayment.AmountPaidString;
                        txtAmountPaid.Focus();
                    }

                    break;
                case "Manifest":
                    DateTime currentDate = DateTime.Now;
                    var entities = shipmentService.FilterActiveBy(x => x.DateAccepted.Year == currentDate.Year && x.DateAccepted.Month == currentDate.Month && x.DateAccepted.Day == currentDate.Day);
                    var shipments = shipmentService.EntitiesToModels(entities);
                    gridManifest.DataSource = ConvertToDataTable(shipments);

                    gridManifest.BestFitColumns(BestFitColumnMode.AllCells);

                    break;
                case "Payment Summary":
                    PaymentSummaryLoadData();
                    PopulateGrid_Prepaid();
                    PopulateGrid_FreightCollect();
                    PopulateGrid_CorpAcctConsignee();
                    gridPrepaid.Columns["Validate"].PinPosition = Telerik.WinControls.UI.PinnedColumnPosition.Right;
                    gridPrepaid.Columns["Client"].PinPosition = Telerik.WinControls.UI.PinnedColumnPosition.Left;

                    gridFreightCollect.Columns["Validate"].PinPosition = Telerik.WinControls.UI.PinnedColumnPosition.Right;
                    gridFreightCollect.Columns["Client"].PinPosition = Telerik.WinControls.UI.PinnedColumnPosition.Left;
                    break;
                case "Tracking":
                    PopulateGrid_FreightCollect();
                    PopulateGrid_CorpAcctConsignee();
                    List<BranchCorpOffice> branchCorpOffices = getBranchCorpOffice();
                    dropDownPickUpCargo_BCO.DataSource = branchCorpOffices;
                    dropDownPickUpCargo_BCO.DisplayMember = "BranchCorpOfficeName";
                    dropDownPickUpCargo_BCO.ValueMember = "BranchCorpOfficeId";
                    dropDownPickUpCargo_BCO.SelectedIndex = -1;
                    dropDownPickUpCargo_BCO.SelectedValue = GlobalVars.DeviceBcoId;

                    List<RevenueUnit> _revenueUnit = getRevenueList();
                    dropDownPickUpCargo_Area.DataSource = _revenueUnit;
                    dropDownPickUpCargo_Area.DisplayMember = "RevenueUnitName";
                    dropDownPickUpCargo_Area.ValueMember = "RevenueUnitId";

                    dropDownPickUpCargo_Area.Items.Add("All");
                    dropDownPickUpCargo_Area.SelectedValue = "All";

                    getPickupCargoData();

                    dateTimePicker_PickupCargo.Value = DateTime.Now;
                    dateTimePickerBranchAcceptance_Date.Value = DateTime.Now;

                    /******** SET COMBOBOX (BRANCH) *******/
                    dropDownBundle_Branch.SelectedIndex = 0;
                    dropDownBranchAcceptance_Branch.SelectedIndex = 0;

                    break;
                default:
                    break;
            }
        }
        private void Main_Resize(object sender, EventArgs e)
        {
        }
        #endregion

        #region Booking

        private void btnRefreshGrid_Click(object sender, EventArgs e)
        {
            PopulateGrid();
        }

        private void lstOriginCity_Enter(object sender, EventArgs e)
        {
            if (lstOriginBco.SelectedIndex >= 0)
            {
                var bcoId = Guid.Parse(lstOriginBco.SelectedValue.ToString());
                List<string> _cities = cities.Where(x => x.BranchCorpOfficeId == bcoId).Select(x => x.CityName).ToList();
                shipperCity = new AutoCompleteStringCollection();
                foreach (var item in _cities)
                {
                    shipperCity.Add(item);
                }
                lstOriginCity.AutoCompleteDataSource = shipperCity;
            }
        }

        private void lstOriginCity_Validated(object sender, EventArgs e)
        {
            txtShipperContactNo.Focus();
        }

        private void lstDestinationBco_Validated(object sender, EventArgs e)
        {
            if (lstDestinationBco.SelectedIndex >= 0)
            {
                var bcoId = Guid.Parse(lstDestinationBco.SelectedValue.ToString());
                SelectedDestinationCity(bcoId);

                lstDestinationCity.Refresh();
                lstDestinationCity.SelectedIndex = -1;
                lstDestinationCity.Focus();
            }
        }

        private void lstDestinationCity_Enter(object sender, EventArgs e)
        {
            if (lstDestinationBco.SelectedIndex > 0)
            {
                var bcoId = Guid.Parse(lstDestinationBco.SelectedValue.ToString());
                List<string> _cities = cities.Where(x => x.BranchCorpOfficeId == bcoId).Select(x => x.CityName).ToList(); consgineeCity = new AutoCompleteStringCollection();
                foreach (var item in _cities)
                {
                    consgineeCity.Add(item);
                }
                lstDestinationCity.AutoCompleteDataSource = consgineeCity;
            }
        }

        private void txtShipperAddress1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperAddress2.Focus();
            }
        }

        private void txtShipperAddress2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperStreet.Focus();
            }
        }

        private void txtShipperStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperBarangay.Focus();
            }
        }

        private void txtShipperBarangay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstOriginBco.Focus();
            }
        }

        private void txtShipperMobile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperEmail.Focus();
            }
        }

        private void txtConsigneeAddress1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeAddress2.Focus();
            }
        }

        private void txtConsigneeAddress2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsgineeStreet.Focus();
            }
        }

        private void txtConsgineeStreet_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeBarangay.Focus();
            }
        }

        private void txtConsigneeBarangay_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstDestinationBco.Focus();
            }
        }

        private void txtConsigneeMobile_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeEmail.Focus();
            }
        }

        private void dateDateBooked_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstAssignedTo.Focus();
            }
        }

        private void lstAssignedTo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstBookingStatus.Focus();
            }
        }

        private void lstAssignedTo_Enter(object sender, EventArgs e)
        {
            assignedTo = new AutoCompleteStringCollection();

            // change to revenue unit under bco only, where bco is in the app settings
            foreach (var item in areas.OrderBy(x => x.RevenueUnitName).Select(x => x.RevenueUnitName).ToList())
            {
                assignedTo.Add(item);
            }
            lstAssignedTo.AutoCompleteDataSource = assignedTo;
        }

        private void lstBookingStatus_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstBookingRemarks.Focus();
            }
        }

        private void lstBookingRemarks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtRemarks.Focus();
            }
        }

        private void txtRemarks_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.Focus();
            }
        }

        private void txtShipperAddress2_Enter(object sender, EventArgs e)
        {
            txtShipperAddress2.SelectAll();
        }

        private void txtShipperStreet_Enter(object sender, EventArgs e)
        {
            txtShipperStreet.SelectAll();
        }

        private void txtShipperBarangay_Enter(object sender, EventArgs e)
        {
            txtShipperBarangay.SelectAll();
        }

        private void txtShipperContactNo_Enter(object sender, EventArgs e)
        {
            txtShipperContactNo.SelectAll();
            //txtShipperContactNo.SelectionStart = 0;
            //txtShipperContactNo.SelectionLength = txtShipperContactNo.Mask.Length;
        }

        private void txtShipperMobile_Enter(object sender, EventArgs e)
        {
            txtShipperMobile.SelectAll();
        }

        private void txtShipperEmail_Enter(object sender, EventArgs e)
        {
            txtShipperEmail.SelectAll();
        }

        private void txtConsigneeAddress2_Enter(object sender, EventArgs e)
        {
            txtConsigneeAddress2.SelectAll();
        }

        private void txtConsgineeStreet_Enter(object sender, EventArgs e)
        {
            txtConsgineeStreet.SelectAll();
        }

        private void txtConsigneeBarangay_Enter(object sender, EventArgs e)
        {
            txtConsigneeBarangay.SelectAll();
        }

        private void txtConsigneeContactNo_Enter(object sender, EventArgs e)
        {
            txtConsigneeContactNo.SelectAll();
        }

        private void txtConsigneeMobile_Enter(object sender, EventArgs e)
        {
            txtConsigneeMobile.SelectAll();
        }

        private void txtConsigneeEmail_Enter(object sender, EventArgs e)
        {
            txtConsigneeEmail.SelectAll();
        }

        private void BookingGridView_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            try
            {
                Guid rowId = Guid.Parse(BookingGridView.Rows[e.RowIndex].Cells["BookingId"].Value.ToString());
                BookingGridView.Rows[e.RowIndex].IsSelected = true;
                BookingSelected(rowId);
                NewShipment();
            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void BookingGridView_CellClick(object sender, GridViewCellEventArgs e)
        {
            try
            {
                Guid rowId = Guid.Parse(BookingGridView.Rows[e.RowIndex].Cells["BookingId"].Value.ToString());
                BookingGridView.Rows[e.RowIndex].IsSelected = true;
                BookingSelected(rowId);
            }
            catch (Exception)
            {
                return;
            }

        }

        private void lstOriginBco_Enter(object sender, EventArgs e)
        {
            shipperBco = new AutoCompleteStringCollection();
            var bcos = branchCorpOffices.OrderBy(x => x.BranchCorpOfficeName).Select(x => x.BranchCorpOfficeName).ToList();
            foreach (var item in bcos)
            {
                shipperBco.Add(item);
            }
            lstOriginBco.AutoCompleteDataSource = shipperBco;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveBooking();
        }

        private void btnSave_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void btnAcceptance_Click(object sender, EventArgs e)
        {
            btnAcceptance.Enabled = false;
            btnSave.Enabled = false;
            NewShipment();
        }

        private void btnAcceptance_KeyUp(object sender, KeyEventArgs e)
        {
            btnAcceptance.Enabled = false;
            btnSave.Enabled = false;
            NewShipment();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            BookingResetAll();
        }

        private void btnReset_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BookingResetAll();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewBooking();
        }

        private void btnNew_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                NewBooking();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteBooking();
        }

        private void btnDelete_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DeleteBooking();
            }
        }

        private void txtShippperLastName_Enter(object sender, EventArgs e)
        {
            shipperLastNames = new AutoCompleteStringCollection();
            var lastnames = clients.OrderBy(x => x.LastName).Select(x => x.LastName).ToList();
            foreach (var item in lastnames)
            {
                shipperLastNames.Add(item);
            }
            txtShipperLastName.AutoCompleteCustomSource = shipperLastNames;
        }

        private void txtShippperLastName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperFirstName.Focus();
            }
        }

        private void txtShipperFirstName_Enter(object sender, EventArgs e)
        {
            shipperFirstNames = new AutoCompleteStringCollection();
            var firstnames = clients.Where(x => x.LastName.Equals(txtShipperLastName.Text.Trim())).OrderBy(x => x.FirstName).Select(x => x.FirstName).ToList();
            foreach (var item in firstnames)
            {
                shipperFirstNames.Add(item);
            }
            txtShipperFirstName.AutoCompleteCustomSource = shipperFirstNames;
        }

        private void txtShipperFirstName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperCompany.Focus();
            }
        }

        private void txtShipperFirstName_Leave(object sender, EventArgs e)
        {
            CreateShipper();
        }

        private void txtShipperCompany_Enter(object sender, EventArgs e)
        {
            shipperCompany = new AutoCompleteStringCollection();
            if (shipper != null)
            {
                var _clients = clients.Where(x => x.LastName.Equals(shipper.LastName) && x.FirstName.Equals(shipper.FirstName)).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
                if (_clients.Count > 0)
                {
                    foreach (var item in _clients)
                    {
                        if (item.CompanyId == null)
                            shipperCompany.Add(item.Company.CompanyName);
                        else
                            shipperCompany.Add(item.Company.CompanyName + " - " + item.AccountNo);
                    }
                }
                else
                {
                    var _companies = companyService.FilterActive().OrderBy(x => x.CompanyName).ToList();
                    foreach (var item in _companies)
                    {
                        if (!string.IsNullOrEmpty(item.CompanyName))
                            shipperCompany.Add(item.CompanyName + " - " + item.AccountNo);
                    }
                }

                txtShipperCompany.AutoCompleteCustomSource = shipperCompany;
            }

        }

        private void txtConsigneeLastName_Enter(object sender, EventArgs e)
        {
            clientConsigneeLastNames = new AutoCompleteStringCollection();
            var lastnames = clients.OrderBy(x => x.LastName).Select(x => x.LastName).ToList();
            foreach (var item in lastnames)
            {
                clientConsigneeLastNames.Add(item);
            }
            txtConsigneeLastName.AutoCompleteCustomSource = clientConsigneeLastNames;
        }

        private void txtConsigneeLastName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeFirstName.Focus();
            }
        }

        private void txtConsigneeFirstName_Enter(object sender, EventArgs e)
        {
            clientConsigneeFirstNames = new AutoCompleteStringCollection();
            var firstnames = clients.Where(x => x.LastName.Equals(txtConsigneeLastName.Text.Trim())).OrderBy(x => x.FirstName).Select(x => x.FirstName).ToList();
            foreach (var item in firstnames)
            {
                clientConsigneeFirstNames.Add(item);
            }
            txtConsigneeFirstName.AutoCompleteCustomSource = clientConsigneeFirstNames;
        }

        private void txtConsigneeFirstName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeCompany.Focus();
            }
        }

        private void txtConsigneeFirstName_Leave(object sender, EventArgs e)
        {
            CreateConsignee();
        }

        private void txtConsigneeCompany_Enter(object sender, EventArgs e)
        {
            consigneeCompany = new AutoCompleteStringCollection();
            if (consignee != null)
            {
                var _clients = clients.Where(x => x.LastName.Equals(consignee.LastName) && x.FirstName.Equals(consignee.FirstName)).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
                if (_clients.Count > 0)
                {
                    foreach (var item in _clients)
                    {
                        if (item.CompanyId == null)
                            consigneeCompany.Add(item.Company.CompanyName);
                        else
                            consigneeCompany.Add(item.Company.CompanyName + " - " + item.AccountNo);
                    }
                }
                else
                {
                    var _companies = companyService.FilterActive().OrderBy(x => x.CompanyName).ToList();
                    foreach (var item in _companies)
                    {
                        if (!string.IsNullOrEmpty(item.CompanyName))
                            consigneeCompany.Add(item.CompanyName + " - " + item.AccountNo);
                    }
                }

                txtConsigneeCompany.AutoCompleteCustomSource = consigneeCompany;
            }

        }

        private void lstOriginBco_Validated(object sender, EventArgs e)
        {
            if (lstOriginBco.SelectedIndex >= 0)
            {
                var bcoId = Guid.Parse(lstOriginBco.SelectedValue.ToString());
                SelectedOriginCity(bcoId);

                lstAssignedTo.DataSource = null;
                lstAssignedTo.Refresh();
                areas = areaService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();
                var _areas = areas.Where(x => x.City.BranchCorpOfficeId == bcoId).ToList();
                lstAssignedTo.DataSource = _areas;
                lstAssignedTo.DisplayMember = "RevenueUnitName";
                lstAssignedTo.ValueMember = "RevenueUnitId";
                assignedTo = new AutoCompleteStringCollection();
                foreach (var item in _areas.OrderBy(x => x.RevenueUnitName).Select(x => x.RevenueUnitName).ToList())
                {
                    assignedTo.Add(item);
                }
                lstAssignedTo.AutoCompleteDataSource = assignedTo;
                lstAssignedTo.SelectedIndex = -1;
                lstOriginCity.SelectedIndex = -1;
                lstOriginCity.Refresh();
                lstOriginCity.Focus();
            }
        }

        private void lstDestinationBco_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (lstDestinationBco.SelectedValue == null)
                {
                    MessageBox.Show("BCO not selected", "Data Error", MessageBoxButtons.OK);
                }
                else
                {
                    var bcoId = Guid.Parse(lstDestinationBco.SelectedValue.ToString());
                    SelectedDestinationCity(bcoId);
                    lstDestinationCity.Focus();
                }
            }
        }

        private void lstOriginCity_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperContactNo.Focus();
            }
        }

        private void lstOriginBco_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (lstOriginBco.SelectedIndex >= 0)
            {
                string n = lstOriginBco.SelectedValue.ToString();
                Guid bcoId = new Guid();
                try
                {
                    bcoId = Guid.Parse(lstOriginBco.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedOriginCity(bcoId);

                lstAssignedTo.DataSource = null;
                lstAssignedTo.Refresh();
                areas = areaService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();
                var _areas = areas.Where(x => x.City.BranchCorpOfficeId == bcoId).ToList();
                lstAssignedTo.DataSource = _areas;
                lstAssignedTo.DisplayMember = "RevenueUnitName";
                lstAssignedTo.ValueMember = "RevenueUnitId";
                assignedTo = new AutoCompleteStringCollection();
                foreach (var item in _areas.OrderBy(x => x.RevenueUnitName).Select(x => x.RevenueUnitName).ToList())
                {
                    assignedTo.Add(item);
                }
                lstAssignedTo.AutoCompleteDataSource = assignedTo;
                lstAssignedTo.SelectedIndex = -1;

                lstOriginCity.Refresh();
                lstOriginCity.Focus();
            }
        }

        private void lstDestinationBco_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (lstDestinationBco.SelectedIndex >= 0)
            {
                Guid bcoId = new Guid();
                try
                {
                    bcoId = Guid.Parse(lstDestinationBco.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedDestinationCity(bcoId);

                //lstAssignedTo.DataSource = null;
                //lstAssignedTo.Refresh();
                //areas = areaService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();
                //var _areas = areas.Where(x => x.City.BranchCorpOfficeId == bcoId).ToList();
                //lstAssignedTo.DataSource = _areas;
                //lstAssignedTo.DisplayMember = "RevenueUnitName";
                //lstAssignedTo.ValueMember = "RevenueUnitId";
                //assignedTo = new AutoCompleteStringCollection();
                //foreach (var item in _areas.OrderBy(x => x.RevenueUnitName).Select(x => x.RevenueUnitName).ToList())
                //{
                //    assignedTo.Add(item);
                //}
                //lstAssignedTo.AutoCompleteDataSource = assignedTo;

                lstDestinationCity.SelectedIndex = -1;
                lstDestinationCity.Refresh();
                lstDestinationCity.Focus();
            }
        }

        private void lstDestinationCity_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeContactNo.Focus();
            }
        }

        private void chkHasDailyBooking_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstAssignedTo.Focus();
            }
        }

        private void txtShipperCompany_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperAddress1.Focus();
            }
        }

        private void txtShipperContactNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtShipperMobile.Focus();
            }
        }

        private void txtShipperEmail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeLastName.Focus();
            }
        }

        private void txtConsigneeCompany_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeAddress1.Focus();
            }
        }

        private void txtConsigneeContactNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtConsigneeMobile.Focus();
            }
        }

        private void txtConsigneeEmail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                dateDateBooked.Focus();
            }
        }

        private void txtShipperEmail_Validating(object sender, CancelEventArgs e)
        {
            if (!(txtShipperEmail.Text == "N/A" || txtShipperEmail.Text == ""))
            {
                bool result = (txtShipperEmail.MaskedEditBoxElement.Provider as EMailMaskTextBoxProvider).Validate(txtShipperEmail.Text);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtConsigneeEmail_Validating(object sender, CancelEventArgs e)
        {
            if (!(txtConsigneeEmail.Text == "N/A" || txtConsigneeEmail.Text == ""))
            {
                bool result = (txtConsigneeEmail.MaskedEditBoxElement.Provider as EMailMaskTextBoxProvider).Validate(txtConsigneeEmail.Text);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtShipperMobile_Validating(object sender, CancelEventArgs e)
        {

            if (!(txtShipperMobile.Text == "(0000) 000-0000"))
            {
                bool result = (txtShipperMobile.Text.Replace("_", "").ToString().Length == txtShipperMobile.Mask.Length);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtShipperContactNo_Validating(object sender, CancelEventArgs e)
        {
            if (!(txtShipperContactNo.Text == "000-0000"))
            {
                bool result = (txtShipperContactNo.Text.Replace("_", "").ToString().Length == txtShipperContactNo.Mask.Length);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtConsigneeMobile_Validating(object sender, CancelEventArgs e)
        {
            if (!(txtConsigneeMobile.Text == "(0000) 000-0000"))
            {
                bool result = (txtConsigneeMobile.Text.Replace("_", "").ToString().Length == txtConsigneeMobile.Mask.Length);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void txtConsigneeContactNo_Validating(object sender, CancelEventArgs e)
        {
            if (!(txtConsigneeContactNo.Text == "000-0000"))
            {
                bool result = (txtConsigneeContactNo.Text.Replace("_", "").ToString().Length == txtConsigneeContactNo.Mask.Length);
                if (!result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            GroupShipper.Enabled = true;
            GroupConsignee.Enabled = true;
            GroupRemarks.Enabled = true;

            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnAcceptance.Enabled = false;
            btnNew.Enabled = false;
            btnReset.Enabled = true;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            if (IsAdmin())
            {
                using (CmsDbCon settings = new CmsDbCon())
                {
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        settings.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("You have insuficient privilege. Please Run as Administrator.","Adminstrator", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }

        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        #endregion

        #region Acceptance

        private void CmsAcceptance_Leave(object sender, EventArgs e)
        {
            AcceptanceResetAll();
        }
        private void CmsAcceptance_Resize(object sender, EventArgs e)
        {
            panelContent.Left = (this.Width - panelContent.Width) / 2;
        }
        private void btnAddPackage_Click(object sender, EventArgs e)
        {
            AddPackage();
        }
        private void btnAddPackage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddPackage();
            }
        }
        private void btnCompute_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                ComputeCharges();
                btnAcceptanceSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Unable to compute.", "Data Validation");
            }

        }
        private void btnCompute_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void AcceptancebtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AcceptancetxtAirwayBill.Text))
            {
                AcceptancetxtAirwayBill.Focus();
                return;
            }
            else
            {
                shipment.AirwayBillNo = AcceptancetxtAirwayBill.Text.Trim();
            }

            btnReset.Enabled = false;
            btnCompute.Enabled = false;
            btnAcceptanceSave.Enabled = false;

            #region CaptureShipmentInput

            if (shipment.ShipmentId == null || shipment.ShipmentId == Guid.Empty)
            {
                shipment.ShipmentId = Guid.NewGuid();
            }

            shipment.CreatedDate = DateTime.Now;
            shipment.CreatedBy = AppUser.User.UserId;
            shipment.LastPaymentDate = null;
            shipment.DateOfFullPayment = null;
            shipment.AcceptedAreaId = AppUser.User.Employee.AssignedToAreaId;
            shipment.AcceptedArea = AppUser.Employee.AssignedToArea;
            shipment.AcceptedById = AppUser.Employee.EmployeeId;
            shipment.AcceptedBy = AppUser.Employee;
            if (shipment.CommodityId == null || shipment.CommodityId == Guid.Empty)
                shipment.CommodityId = Guid.Parse(lstCommodity.SelectedValue.ToString());
            shipment.Notes = txtNotes.Text;
            shipment.GoodsDescriptionId = Guid.Parse(lstGoodsDescription.SelectedValue.ToString());

            //shipment.DestinationCityId = Guid.Parse(lstDestinationCity.SelectedValue.ToString());
            shipment.ModifiedBy = AppUser.User.UserId;
            shipment.ModifiedDate = DateTime.Now;
            shipment.RecordStatus = (int)RecordStatus.Active;

            // TOO this is default payment term
            shipment.PaymentTermId = paymentTerms.Find(x => x.PaymentTermName.Equals("Cash")).PaymentTermId;
            if (shipment.PaymentMode.PaymentModeCode.Equals("PP"))
            {
                shipment.PaymentTermId = paymentTerms.Find(x => x.PaymentTermName.Equals("Cash")).PaymentTermId;
            }
            else if (shipment.PaymentMode.PaymentModeCode.Equals("FC"))
            {
                shipment.PaymentTermId = paymentTerms.Find(x => x.PaymentTermName.Equals("COD")).PaymentTermId;
            }
            else
            {
                if (shipment.Consignee.Company != null && shipment.Consignee.CompanyId != null)
                {
                    if (shipment.PaymentMode.PaymentModeCode.Equals("CAC"))
                    {
                        shipment.PaymentTermId = shipment.Consignee.Company.PaymentTerm.PaymentTermId;
                    }
                }

                if (shipment.Shipper.Company != null && shipment.Shipper.CompanyId != null)
                {
                    if (shipment.PaymentMode.PaymentModeCode.Equals("CAS"))
                    {
                        shipment.PaymentTermId = shipment.Shipper.Company.PaymentTerm.PaymentTermId;
                    }
                }
            }

            #region ShipmentPackages

            foreach (var item in shipment.PackageDimensions)
            {
                item.ShipmentId = shipment.ShipmentId;
                item.CreatedBy = AppUser.User.UserId;
                item.CreatedDate = DateTime.Now;
                item.ModifiedBy = AppUser.User.UserId;
                item.ModifiedDate = DateTime.Now;
                item.RecordStatus = (int)RecordStatus.Active;
            }

            #endregion

            #endregion

            ProgressIndicator saving = new ProgressIndicator("Acceptance", "Saving ...", SaveShipment);
            saving.ShowDialog();

            shipmentModel = shipment;
            btnAcceptanceReset.Enabled = true;
            btnPrint.Enabled = true;
            btnPayment.Enabled = true;
            btnSearchShipment.Enabled = true;

            //PrintAwb();

            //if (shipment.PaymentMode.PaymentModeCode.Equals("PP"))
            //{
            //    ProceedToPayment();
            //}

        }
        private void AcceptancebtnPrint_Click(object sender, EventArgs e)
        {
            PrintAwb();
        }
        private void AcceptancebtnReset_Click(object sender, EventArgs e)
        {
            ClearSummaryData();
            AcceptanceResetAll();
            isNewShipment = false;
        }
        private void lstCommodityType_Validated(object sender, EventArgs e)
        {
            CommodityTypeSelected();
        }
        private void txtWeight_KeyUp(object sender, KeyEventArgs e)
        {

        }
        private void txtLength_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtWidth.Focus();
            }
        }
        private void txtWidth_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtHeight.Focus();
            }
        }
        private void txtHeight_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAddPackage.Focus();
            }
        }
        private void dateAcceptedDate_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AcceptancetxtAirwayBill.Focus();
            }
        }
        private void txtAirwayBill_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (isNewShipment)
                {
                    if (IsNumericOnly(8, 8, AcceptancetxtAirwayBill.Text.Trim()))
                    {
                        if (shipmentService.GetAll().Where(x => x.AirwayBillNo == AcceptancetxtAirwayBill.Text).Count() > 0)
                        {
                            MessageBox.Show("Airwaybill number already exist.", "Search Airwaybill Number", MessageBoxButtons.OK);
                            return;
                        }
                        lstCommodityType.Focus();
                        lstCommodityType.SelectedIndex = -1;
                        EnableForm();
                    }
                    else
                    {
                        MessageBox.Show("Invalid AirwayBill No.", "Data Error", MessageBoxButtons.OK);
                        AcceptancetxtAirwayBill.Focus();
                    }
                }
                else
                {
                    btnSearchShipment.PerformClick();
                }

            }
        }
        private void txtDeclaredValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }
        }
        private void chkNonVatable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

            }
        }
        private void btnSearchShipment_Click(object sender, EventArgs e)
        {
            List<Shipment> _shipment = shipmentService.FilterActiveBy(x => x.AirwayBillNo.Equals(AcceptancetxtAirwayBill.Text.ToString()));

            if (_shipment != null && _shipment.Count > 0)
            {
                AcceptanceLoadData();

                shipment = shipmentService.EntityToModel(_shipment.FirstOrDefault());
                commodityType = commodityTypes.Find(x => x.CommodityTypeId == shipment.CommodityTypeId);

                AcceptancePopulateForm();
                RefreshGridPackages();
                PopulateSummary();

                btnReset.Enabled = true;
                btnCompute.Enabled = false;
                btnAcceptanceSave.Enabled = false;
                btnPrint.Enabled = false;
                btnAcceptanceEdit.Enabled = true;


            }
            else
            {
                MessageBox.Show("No record found.", "Airwaybill Search");
            }

            shipmentModel = shipment;
        }
        private void AcceptancebtnPayment_Click(object sender, EventArgs e)
        {
            if (shipment.PaymentMode.PaymentModeCode.Equals("PP"))
            {
                //btnPayment.Enabled = false;
                ProceedToPayment();
            }
        }
        private void lstServiceMode_Validated(object sender, EventArgs e)
        {
            if (lstServiceMode.SelectedIndex > -1)
            {
                shipment.ServiceModeId = Guid.Parse(lstServiceMode.SelectedValue.ToString());
                shipment.ServiceMode = serviceModes.FirstOrDefault(x => x.ServiceModeId == shipment.ServiceTypeId);
                RefreshGridPackages();
                RefreshOptions();
            }

        }
        private void txtWeight_Validated(object sender, EventArgs e)
        {
            RefreshGridPackages();
        }
        private void lstCommodity_Enter(object sender, EventArgs e)
        {
            if (lstCommodityType.SelectedIndex > -1)
            {
                var commodityTypeId = Guid.Parse(lstCommodityType.SelectedValue.ToString());
                commodityCollection = new AutoCompleteStringCollection();
                foreach (
                    var item in
                        commodities.Where(x => x.CommodityTypeId == commodityTypeId)
                            .OrderBy(x => x.CommodityName)
                            .Select(x => x.CommodityName)
                            .ToList())
                {
                    commodityCollection.Add(item);
                }
                lstCommodity.AutoCompleteDataSource = commodityCollection;
            }

        }
        private void lstServiceMode_Enter(object sender, EventArgs e)
        {

            serviceModeCollection = new AutoCompleteStringCollection();
            foreach (var item in serviceModes.OrderBy(x => x.ServiceModeName).Select(x => x.ServiceModeName).ToList())
            {
                serviceModeCollection.Add(item);
            }
            lstServiceMode.AutoCompleteDataSource = serviceModeCollection;
        }
        private void lstShipMode_Enter(object sender, EventArgs e)
        {
            shipModeCollection = new AutoCompleteStringCollection();
            foreach (var item in shipModes.OrderBy(x => x.ShipModeName).Select(x => x.ShipModeName).ToList())
            {
                shipModeCollection.Add(item);
            }
            lstShipMode.AutoCompleteDataSource = shipModeCollection;
        }
        private void lstGoodsDescription_Enter(object sender, EventArgs e)
        {
            goodsDescCollection = new AutoCompleteStringCollection();
            foreach (
                var item in
                    goodsDescriptions.OrderBy(x => x.GoodsDescriptionName).Select(x => x.GoodsDescriptionName).ToList())
            {
                goodsDescCollection.Add(item);
            }
            lstGoodsDescription.AutoCompleteDataSource = goodsDescCollection;
        }
        private void lstPaymentMode_Enter(object sender, EventArgs e)
        {
            paymentModeCollection = new AutoCompleteStringCollection();
            foreach (var item in paymentModes.OrderBy(x => x.PaymentModeName).Select(x => x.PaymentModeName).ToList())
            {
                paymentModeCollection.Add(item);
            }
            lstPaymentMode.AutoCompleteDataSource = paymentModeCollection;
        }
        private void gridPackage_UserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {

            int index = Convert.ToInt32(e.Rows[0].Cells["No"].Value) - 1; ;
            PackageDimensionModel item = shipment.PackageDimensions.FirstOrDefault(x => x.Index == index);
            shipment.PackageDimensions.Remove(item);

            RefreshGridPackages();
        }
        private void AcceptancetxtAirwayBill_Enter(object sender, EventArgs e)
        {
            AirwayBill = new AutoCompleteStringCollection();
            List<string> awbs = shipmentService.GetAll().Select(x => x.AirwayBillNo).ToList();
            foreach (string item in awbs)
            {
                AirwayBill.Add(item);
            }
            AcceptancetxtAirwayBill.AutoCompleteCustomSource = AirwayBill;
        }
        private void EditAcceptance_Click(object sender, EventArgs e)
        {

            EnableForm();

            btnAcceptanceEdit.Enabled = false;
            btnCompute.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = true;
            btnPayment.Enabled = true;
            btnReset.Enabled = true;
        }
        private void lstCommodityType_Enter(object sender, EventArgs e)
        {
            commodityTypeCollection = new AutoCompleteStringCollection();
            var ctypes = commodityTypes.OrderBy(x => x.CommodityTypeName).Where(x => x.RecordStatus == 1).Select(x => x.CommodityTypeName).ToList();
            foreach (var item in ctypes)
            {
                commodityTypeCollection.Add(item);
            }
            lstCommodityType.AutoCompleteDataSource = commodityTypeCollection;
        }
        private void lstServiceType_Enter(object sender, EventArgs e)
        {
            serviceTypeCollection = new AutoCompleteStringCollection();
            var servicetype = serviceTypes.OrderBy(x => x.ServiceTypeName).Where(x => x.RecordStatus == 1).Select(x => x.ServiceTypeName).ToList();
            foreach (var item in servicetype)
            {
                serviceTypeCollection.Add(item);
            }
            lstServiceType.AutoCompleteDataSource = serviceTypeCollection;
        }
        private void AcceptancetxtAirwayBill_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void lstPaymentMode_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (shipment == null) return;

            if (lstPaymentMode.SelectedIndex > -1)
            {
                shipment.PaymentMode = paymentModes.Find(x => x.PaymentModeName == lstPaymentMode.SelectedItem.ToString());
                if (shipment.PaymentMode != null)
                {
                    shipment.PaymentModeId = shipment.PaymentMode.PaymentModeId;
                }
            }
        }
        #endregion

        #region Payment

        private void txtSoaNo_TextChanged(object sender, EventArgs e)
        {
            txtAwb.Enabled = false;
            txtAwb.Text = "";
        }

        private void txtAwb_TextChanged(object sender, EventArgs e)
        {
            txtSoaNo.Enabled = false;
            txtSoaNo.Text = "";
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSoaNo.Text))
            {
                // SOA Payment
                var soa = soaService.FilterActiveBy(x => x.StatementOfAccountNo.Equals(txtSoaNo.Text))
                        .FirstOrDefault();
                if (soa != null)
                {
                    soaPayment = new StatementOfAccountPayment();
                    soaPayment.StatementOfAccountPaymentId = Guid.NewGuid();
                    soaPayment.StatementOfAccountId = soa.StatementOfAccountId;
                    soaPayment.OrNo = txtOrNo.Text.Trim();
                    soaPayment.PrNo = txtPrNo.Text.Trim();
                    soaPayment.PaymentDate = datePaymentDate.Value;
                    soaPayment.Amount = decimal.Parse(txtAmountPaid.Text.Trim());
                    soaPayment.PaymentTypeId = Guid.Parse(lstPaymentType.SelectedValue.ToString());
                    if (lstPaymentType.SelectedText == "Check")
                    {
                        soaPayment.CheckBankName = txtCheckBank.Text.Trim();
                        soaPayment.CheckDate = dateCheckDate.Value;
                        soaPayment.CheckNo = txtCheckNo.Text.Trim();
                    }
                    soaPayment.ReceivedById = AppUser.Employee.EmployeeId;
                    soaPayment.Remarks = cmb_PaymentRemarks.Text;
                    soaPayment.CreatedBy = AppUser.User.UserId;
                    soaPayment.CreatedDate = DateTime.Now;
                    soaPayment.ModifiedBy = AppUser.User.UserId;
                    soaPayment.ModifiedDate = DateTime.Now;
                    soaPayment.RecordStatus = (int)RecordStatus.Active;

                }
                else
                {
                    MessageBox.Show("Invalid SOA No", "Data Error", MessageBoxButtons.OK);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(txtAwb.Text.Trim()))
            {
                // AWb Payment
                var shipment = shipmentService.FilterActiveBy(x => x.AirwayBillNo.Equals(txtAwb.Text.Trim()))
                        .FirstOrDefault();
                if (shipment != null)
                {
                    payment = new Payment();
                    payment.PaymentId = Guid.NewGuid();
                    payment.ShipmentId = shipment.ShipmentId;
                    payment.OrNo = txtOrNo.Text.Trim();
                    payment.PrNo = txtPrNo.Text.Trim();
                    payment.PaymentDate = datePaymentDate.Value;
                    try
                    {
                        payment.Amount = decimal.Parse(txtAmountPaid.Value.ToString().Replace("₱", ""));
                        payment.TaxWithheld = decimal.Parse(txtTaxWithheld.Value.ToString().Replace("₱", ""));
                    }
                    catch (Exception)
                    {
                        payment.Amount = decimal.Parse(txtAmountPaid.Value.ToString().Replace("Php", ""));
                        payment.TaxWithheld = decimal.Parse(txtTaxWithheld.Value.ToString().Replace("Php", ""));
                    }

                    payment.PaymentTypeId = Guid.Parse(lstPaymentType.SelectedValue.ToString());
                    if (lstPaymentType.SelectedText == "Check")
                    {
                        payment.CheckBankName = txtCheckBank.Text.Trim();
                        payment.CheckDate = dateCheckDate.Value;
                        payment.CheckNo = txtCheckNo.Text.Trim();
                    }
                    payment.ReceivedById = AppUser.Employee.EmployeeId;
                    payment.Remarks = cmb_PaymentRemarks.Text;
                    payment.CreatedBy = AppUser.User.UserId;
                    payment.CreatedDate = DateTime.Now;
                    payment.ModifiedBy = AppUser.User.UserId;
                    payment.ModifiedDate = DateTime.Now;
                    payment.RecordStatus = (int)RecordStatus.Active;
                }
                else
                {
                    MessageBox.Show("Invalid AWB No", "Data Error", MessageBoxButtons.OK);
                    return;
                }
            }

            btnAccept.Enabled = false;
            btnCancel.Enabled = false;

            ProgressIndicator saving = new ProgressIndicator("Payment", "Saving ...", SavePayment);
            saving.ShowDialog();

            //ProgressIndicator uploading = new ProgressIndicator("Payment", "Uploading ...", UploadToCentral);
            //uploading.ShowDialog();

            btnAccept.Enabled = true;
            btnCancel.Enabled = true;
            PaymentReset();

            shipmentModel = null;
            isNewShipment = false;

            NewPayment = null;

            AcceptanceResetAll();
            ClearSummaryData();
            DisableForm();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PaymentReset();
        }

        private void txtAmountPaid_Leave(object sender, EventArgs e)
        {

        }

        private void txtTaxWithheld_Leave(object sender, EventArgs e)
        {
            ComputeNetCollection();
        }

        private void lstPaymentType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (lstPaymentType.SelectedText == "Check")
            {
                txtCheckBank.Enabled = true;
                txtCheckNo.Enabled = true;
                dateCheckDate.Enabled = true;
                txtCheckBank.Focus();
            }
            else
            {
                txtCheckBank.Enabled = false;
                txtCheckNo.Enabled = false;
                dateCheckDate.Enabled = false;
                txtRemarks.Focus();
            }
        }

        private void txtAwb_Enter(object sender, EventArgs e)
        {
            AirwayBill = new AutoCompleteStringCollection();
            List<string> awbs = shipmentService.GetAll().Select(x => x.AirwayBillNo).ToList();
            foreach (string item in awbs)
            {
                AirwayBill.Add(item);
            }
            txtAwb.AutoCompleteCustomSource = AirwayBill;
        }

        private void txtAwb_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                List<Shipment> _shipment = shipmentService.FilterActiveBy(x => x.AirwayBillNo.Equals(txtAwb.Text.ToString()));
                if (_shipment != null && _shipment.Count > 0)
                {
                    shipment = shipmentService.EntityToModel(_shipment.FirstOrDefault());
                    LoadPayment();
                }
                else
                {
                    MessageBox.Show("No record found.", "Airwaybill Search");
                }

            }

            shipmentModel = shipment;
        }
        #endregion

        #region Payment Summary


        private void btnSearch_Click(object sender, EventArgs e)
        {
            PopulateGrid_Prepaid();
            PopulateGrid_FreightCollect();
            PopulateGrid_CorpAcctConsignee();
            TotalPaymentSummary();
            clearPaymentSummaryData();
            chk_ReceivedAll.Checked = false;
            listPaymentSummary = new List<PaymentSummaryModel>();
        }

        private void lstRevenueUnitType_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex >= 0)
            {
                Guid revenueUnitTypeId = new Guid();
                try
                {
                    revenueUnitTypeId = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedRevenueUnit(revenueUnitTypeId);
            }
        }

        private void lstRevenueUnitName_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

            if (lstRevenueUnitName.SelectedIndex >= 0)
            {
                Guid revenueUnitId = new Guid();
                try
                {
                    revenueUnitId = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedUser(revenueUnitId);
            }
        }

        private void gridPrepaid_ValueChanged(object sender, EventArgs e)
        {
            int rowIndex_On;

            //properties
            string awbsoa;
            string clientName;
            string paymentType;
            decimal amountDue;
            decimal amountPaid;
            decimal taxWithheld;
            string OrNo;
            string PrNo;
            string ValidatedBy;


            Guid clientId = new Guid();
            Guid paymentId = new Guid();
            Guid validatedById = new Guid();
            string paymentModecode = "PP";
            if (this.gridPrepaid.ActiveEditor is RadCheckBoxEditor)
            {
                Console.WriteLine(this.gridPrepaid.CurrentCell.RowIndex);
                rowIndex_On = this.gridPrepaid.CurrentCell.RowIndex;
                Console.WriteLine(this.gridPrepaid.ActiveEditor.Value.ToString());
                string value = this.gridPrepaid.ActiveEditor.Value.ToString();
                if (value.Equals("On"))
                {

                    awbsoa = gridPrepaid.Rows[rowIndex_On].Cells["AWB No"].Value.ToString();
                    clientName = gridPrepaid.Rows[rowIndex_On].Cells["Client"].Value.ToString();
                    amountDue = Convert.ToDecimal(gridPrepaid.Rows[rowIndex_On].Cells["Amount Due"].Value);
                    taxWithheld = Convert.ToDecimal(gridPrepaid.Rows[rowIndex_On].Cells["Tax Withheld"].Value);
                    OrNo = gridPrepaid.Rows[rowIndex_On].Cells["OR No"].Value.ToString();
                    PrNo = gridPrepaid.Rows[rowIndex_On].Cells["PR No"].Value.ToString();
                    ValidatedBy = gridPrepaid.Rows[rowIndex_On].Cells["Validated By"].Value.ToString();


                    amountPaid = Convert.ToDecimal(gridPrepaid.Rows[rowIndex_On].Cells["Amount Paid"].Value);
                    paymentType = gridPrepaid.Rows[rowIndex_On].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridPrepaid.Rows[rowIndex_On].Cells["ClientId"].Value.ToString());
                    paymentId = Guid.Parse(gridPrepaid.Rows[rowIndex_On].Cells["PaymentId"].Value.ToString());
                    validatedById = Guid.Parse(gridPrepaid.Rows[rowIndex_On].Cells["ValidatedById"].Value.ToString());
                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived += amountPaid;
                    }
                    else
                    {
                        totalCheckReceived += amountPaid;
                    }

                    listofPaymentSummary(clientId, paymentId, validatedById, paymentModecode);
                    summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModecode);
                }
                else
                {
                    amountPaid = Convert.ToDecimal(gridPrepaid.Rows[rowIndex_On].Cells["Amount Paid"].Value);
                    paymentType = gridPrepaid.Rows[rowIndex_On].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridPrepaid.Rows[rowIndex_On].Cells["ClientId"].Value.ToString());
                    awbsoa = gridPrepaid.Rows[rowIndex_On].Cells["AWB No"].Value.ToString();
                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived -= amountPaid;

                    }
                    else
                    {
                        totalCheckReceived -= amountPaid;

                    }

                    var itemToRemove = listPaymentSummary.Find(r => r.ClientId == clientId);
                    if (itemToRemove != null)
                        listPaymentSummary.Remove(itemToRemove);

                    var removeDetails = listpaymentSummaryDetails.Find(r => r.AwbNo == awbsoa);
                    if (removeDetails != null)
                        listpaymentSummaryDetails.Remove(removeDetails);

                }

                totalAmountReceived = totalCashReceived + totalCheckReceived;
                difference = totalCollection - totalAmountReceived;
                txtTotalCashReceived.Text = totalCashReceived.ToString();
                txtTotalCheckReceived.Text = totalCheckReceived.ToString();
                txtTotalAmntReceived.Text = totalAmountReceived.ToString();
                txtDifference.Text = difference.ToString();
            }
        }

        private void gridFreightCollect_ValueChanged(object sender, EventArgs e)
        {
            int rowIndex_On;

            //properties
            string awbsoa;
            string clientName;
            string paymentType;
            decimal amountDue;
            decimal amountPaid;
            decimal taxWithheld;
            string OrNo;
            string PrNo;
            string ValidatedBy;

            Guid clientId = new Guid();
            Guid paymentId = new Guid();
            Guid validatedById = new Guid();
            string paymentModecode = "FC";
            if (this.gridFreightCollect.ActiveEditor is RadCheckBoxEditor)
            {
                rowIndex_On = this.gridFreightCollect.CurrentCell.RowIndex;
                string value = this.gridFreightCollect.ActiveEditor.Value.ToString();
                if (value.Equals("On"))
                {

                    awbsoa = gridFreightCollect.Rows[rowIndex_On].Cells["AWB No"].Value.ToString();
                    clientName = gridFreightCollect.Rows[rowIndex_On].Cells["Client"].Value.ToString();
                    amountDue = Convert.ToDecimal(gridFreightCollect.Rows[rowIndex_On].Cells["Amount Due"].Value);
                    taxWithheld = Convert.ToDecimal(gridFreightCollect.Rows[rowIndex_On].Cells["Tax Withheld"].Value);
                    OrNo = gridFreightCollect.Rows[rowIndex_On].Cells["OR No"].Value.ToString();
                    PrNo = gridFreightCollect.Rows[rowIndex_On].Cells["PR No"].Value.ToString();
                    ValidatedBy = gridFreightCollect.Rows[rowIndex_On].Cells["Validated By"].Value.ToString();

                    amountPaid = Convert.ToDecimal(gridFreightCollect.Rows[rowIndex_On].Cells["Amount Paid"].Value);
                    paymentType = gridFreightCollect.Rows[rowIndex_On].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridFreightCollect.Rows[rowIndex_On].Cells["ClientId"].Value.ToString());
                    paymentId = Guid.Parse(gridFreightCollect.Rows[rowIndex_On].Cells["PaymentId"].Value.ToString());
                    validatedById = Guid.Parse(gridFreightCollect.Rows[rowIndex_On].Cells["ValidatedById"].Value.ToString());

                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived += amountPaid;

                    }
                    else
                    {
                        totalCheckReceived += amountPaid;
                    }
                    listofPaymentSummary(clientId, paymentId, validatedById, paymentModecode);
                    summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModecode);
                }
                else
                {
                    amountPaid = Convert.ToDecimal(gridFreightCollect.Rows[rowIndex_On].Cells["Amount Paid"].Value);
                    paymentType = gridFreightCollect.Rows[rowIndex_On].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridFreightCollect.Rows[rowIndex_On].Cells["ClientId"].Value.ToString());
                    awbsoa = gridPrepaid.Rows[rowIndex_On].Cells["AWB No"].Value.ToString();

                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived -= amountPaid;

                    }
                    else
                    {
                        totalCheckReceived -= amountPaid;

                    }

                    var itemToRemove = listPaymentSummary.Find(r => r.ClientId == clientId);
                    if (itemToRemove != null)
                        listPaymentSummary.Remove(itemToRemove);

                    var removeDetails = listpaymentSummaryDetails.Find(r => r.AwbNo == awbsoa);
                    if (removeDetails != null)
                        listpaymentSummaryDetails.Remove(removeDetails);

                }

                totalAmountReceived = totalCashReceived + totalCheckReceived;
                difference = totalCollection - totalAmountReceived;
                txtTotalCashReceived.Text = totalCashReceived.ToString();
                txtTotalCheckReceived.Text = totalCheckReceived.ToString();
                txtTotalAmntReceived.Text = totalAmountReceived.ToString();
                txtDifference.Text = difference.ToString();
            }
        }

        private void btnReceivedAll_Click(object sender, EventArgs e)
        {
            clearPaymentSummaryData();
            string awbsoa;
            string clientName;
            string paymentType;
            decimal amountDue;
            decimal amountPaid;
            decimal taxWithheld;
            string OrNo;
            string PrNo;
            string ValidatedBy;

            Guid clientId = new Guid();
            Guid paymentId = new Guid();
            Guid validatedById = new Guid();
            string paymentModeCode;
            for (int i = 0; i < gridPrepaid.Rows.Count; i++)
            {
                gridPrepaid.Rows[i].Cells["Validate"].Value = true;
                awbsoa = gridPrepaid.Rows[i].Cells["AWB No"].Value.ToString();
                clientName = gridPrepaid.Rows[i].Cells["Client"].Value.ToString();
                amountDue = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Amount Due"].Value);
                taxWithheld = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Tax Withheld"].Value);
                OrNo = gridPrepaid.Rows[i].Cells["OR No"].Value.ToString();
                PrNo = gridPrepaid.Rows[i].Cells["PR No"].Value.ToString();
                ValidatedBy = gridPrepaid.Rows[i].Cells["Validated By"].Value.ToString();
                paymentModeCode = "PP";

                amountPaid = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Amount Paid"].Value);
                paymentType = gridPrepaid.Rows[i].Cells["Payment Type"].Value.ToString();
                clientId = Guid.Parse(gridPrepaid.Rows[i].Cells["ClientId"].Value.ToString());
                paymentId = Guid.Parse(gridPrepaid.Rows[i].Cells["PaymentId"].Value.ToString());
                validatedById = Guid.Parse(gridPrepaid.Rows[i].Cells["ValidatedById"].Value.ToString());

                if (paymentType.Equals("Cash"))
                {
                    totalCashReceived += amountPaid;
                }
                else
                {
                    totalCheckReceived += amountPaid;
                }

                listofPaymentSummary(clientId, paymentId, validatedById, paymentModeCode);
                summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModeCode);
            }

            for (int j = 0; j < gridFreightCollect.Rows.Count; j++)
            {
                gridFreightCollect.Rows[j].Cells["Validate"].Value = true;

                awbsoa = gridFreightCollect.Rows[j].Cells["AWB No"].Value.ToString();
                clientName = gridFreightCollect.Rows[j].Cells["Client"].Value.ToString();
                amountDue = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Amount Due"].Value);
                taxWithheld = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Tax Withheld"].Value);
                OrNo = gridFreightCollect.Rows[j].Cells["OR No"].Value.ToString();
                PrNo = gridFreightCollect.Rows[j].Cells["PR No"].Value.ToString();
                ValidatedBy = gridFreightCollect.Rows[j].Cells["Validated By"].Value.ToString();
                paymentModeCode = "FC";


                amountPaid = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Amount Paid"].Value);
                paymentType = gridFreightCollect.Rows[j].Cells["Payment Type"].Value.ToString();
                clientId = Guid.Parse(gridFreightCollect.Rows[j].Cells["ClientId"].Value.ToString());
                paymentId = Guid.Parse(gridFreightCollect.Rows[j].Cells["PaymentId"].Value.ToString());
                validatedById = Guid.Parse(gridFreightCollect.Rows[j].Cells["ValidatedById"].Value.ToString());

                if (paymentType.Equals("Cash"))
                {
                    totalCashReceived += amountPaid;
                }
                else
                {
                    totalCheckReceived += amountPaid;
                }

                listofPaymentSummary(clientId, paymentId, validatedById, paymentModeCode);
                summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModeCode);
            }

            totalAmountReceived = totalCashReceived + totalCheckReceived;
            txtTotalCashReceived.Text = totalCashReceived.ToString();
            txtTotalCheckReceived.Text = totalCheckReceived.ToString();
            txtTotalAmntReceived.Text = totalAmountReceived.ToString();
        }

        private void btnSavePaymentSummary_Click(object sender, EventArgs e)
        {
            SavepaymentSummary(listPaymentSummary);
            amountPaymentSummary();
            ProgressIndicator saving = new ProgressIndicator("Payment Summary", "Saving ...", SavingofPaymentSummary);
            saving.ShowDialog();
            clearSummaryData();
            clearPaymentSummaryData();
            PopulateGrid_Prepaid();
            PopulateGrid_FreightCollect();
            PopulateGrid_CorpAcctConsignee();
            TotalPaymentSummary();
            btnPrintPaymentSummary.Enabled = true;
            chk_ReceivedAll.Checked = false;
        }

        private void img_Signature_MouseDown(object sender, MouseEventArgs e)
        {
            _Previous = e.Location;
            img_Signature_MouseMove(sender, e);
        }

        private void img_Signature_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Previous != null)
            {
                if (img_Signature.Image == null)
                {
                    Bitmap bmp = new Bitmap(img_Signature.Width, img_Signature.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(System.Drawing.Color.White);
                    }
                    img_Signature.Image = bmp;
                    signatureImage = img_Signature.Image;
                }
                using (Graphics g = Graphics.FromImage(img_Signature.Image))
                {
                    //g.DrawLine(Pens.Black, _Previous.Value, e.Location);
                    g.DrawLine(new Pen(System.Drawing.Color.Black, 2), _Previous.Value, e.Location);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }
                img_Signature.Invalidate(); //refreshes the picturebox
                _Previous = e.Location;//keep assigning the _Previous to the current mouse position
            }
        }

        private void img_Signature_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine(img_Signature.Image);
            _Previous = null;
        }

        private void btnPrintPaymentSummary_Click(object sender, EventArgs e)
        {
            PaymentSummaryForm psummaryForm = new PaymentSummaryForm();
            psummaryForm.passListofSummaryDetails(listpaymentSummaryDetails);
            psummaryForm.passListofMainDetails(listMainDetails);
            psummaryForm.Show();
            clearListofPaymentSummary();

        }

        private void btnReset_CancelPaymentSummary_Click(object sender, EventArgs e)
        {
            clearSummaryData();
        }

        private void chk_ReceivedAll_CheckStateChanged(object sender, EventArgs e)
        {
            clearPaymentSummaryData();
            string awbsoa;
            string clientName;
            string paymentType;
            decimal amountDue;
            decimal amountPaid;
            decimal taxWithheld;
            string OrNo;
            string PrNo;
            string ValidatedBy;

            Guid clientId = new Guid();
            Guid paymentId = new Guid();
            Guid validatedById = new Guid();
            string paymentModeCode;

            if (chk_ReceivedAll.Checked)
            {
                #region Prepaid Payment

                for (int i = 0; i < gridPrepaid.Rows.Count; i++)
                {
                    gridPrepaid.Rows[i].Cells["Validate"].Value = true;
                    awbsoa = gridPrepaid.Rows[i].Cells["AWB No"].Value.ToString();
                    clientName = gridPrepaid.Rows[i].Cells["Client"].Value.ToString();
                    amountDue = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Amount Due"].Value);
                    taxWithheld = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Tax Withheld"].Value);
                    OrNo = gridPrepaid.Rows[i].Cells["OR No"].Value.ToString();
                    PrNo = gridPrepaid.Rows[i].Cells["PR No"].Value.ToString();
                    ValidatedBy = gridPrepaid.Rows[i].Cells["Validated By"].Value.ToString();
                    paymentModeCode = "PP";

                    amountPaid = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Amount Paid"].Value);
                    paymentType = gridPrepaid.Rows[i].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridPrepaid.Rows[i].Cells["ClientId"].Value.ToString());
                    paymentId = Guid.Parse(gridPrepaid.Rows[i].Cells["PaymentId"].Value.ToString());
                    validatedById = Guid.Parse(gridPrepaid.Rows[i].Cells["ValidatedById"].Value.ToString());

                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived += amountPaid;
                    }
                    else
                    {
                        totalCheckReceived += amountPaid;
                    }

                    listofPaymentSummary(clientId, paymentId, validatedById, paymentModeCode);
                    summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModeCode);
                }

                #endregion

                #region Freight Collect
                for (int j = 0; j < gridFreightCollect.Rows.Count; j++)
                {
                    gridFreightCollect.Rows[j].Cells["Validate"].Value = true;

                    awbsoa = gridFreightCollect.Rows[j].Cells["AWB No"].Value.ToString();
                    clientName = gridFreightCollect.Rows[j].Cells["Client"].Value.ToString();
                    amountDue = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Amount Due"].Value);
                    taxWithheld = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Tax Withheld"].Value);
                    OrNo = gridFreightCollect.Rows[j].Cells["OR No"].Value.ToString();
                    PrNo = gridFreightCollect.Rows[j].Cells["PR No"].Value.ToString();
                    ValidatedBy = gridFreightCollect.Rows[j].Cells["Validated By"].Value.ToString();
                    paymentModeCode = "FC";


                    amountPaid = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Amount Paid"].Value);
                    paymentType = gridFreightCollect.Rows[j].Cells["Payment Type"].Value.ToString();
                    clientId = Guid.Parse(gridFreightCollect.Rows[j].Cells["ClientId"].Value.ToString());
                    paymentId = Guid.Parse(gridFreightCollect.Rows[j].Cells["PaymentId"].Value.ToString());
                    validatedById = Guid.Parse(gridFreightCollect.Rows[j].Cells["ValidatedById"].Value.ToString());

                    if (paymentType.Equals("Cash"))
                    {
                        totalCashReceived += amountPaid;
                    }
                    else
                    {
                        totalCheckReceived += amountPaid;
                    }

                    listofPaymentSummary(clientId, paymentId, validatedById, paymentModeCode);
                    summaryDetails(awbsoa, clientName, paymentType, amountDue, amountPaid, taxWithheld, OrNo, PrNo, ValidatedBy, paymentModeCode);
                }

                #endregion

            }
            else
            {
                #region Prepaid
                for (int i = 0; i < gridPrepaid.Rows.Count; i++)
                {
                    gridPrepaid.Rows[i].Cells["Validate"].Value = false;
                    awbsoa = gridPrepaid.Rows[i].Cells["AWB No"].Value.ToString();
                    clientId = Guid.Parse(gridPrepaid.Rows[i].Cells["ClientId"].Value.ToString());
                    paymentType = gridPrepaid.Rows[i].Cells["Payment Type"].Value.ToString();
                    amountPaid = Convert.ToDecimal(gridPrepaid.Rows[i].Cells["Amount Paid"].Value);

                    //if (paymentType.Equals("Cash"))
                    //{
                    //    totalCashReceived -= amountPaid;
                    //}
                    //else
                    //{
                    //    totalCheckReceived -= amountPaid;
                    //}


                    var itemRemovePrepaid = listPaymentSummary.Find(r => r.ClientId == clientId);
                    if (itemRemovePrepaid != null)
                        listPaymentSummary.Remove(itemRemovePrepaid);

                    var removePrepaidDetails = listpaymentSummaryDetails.Find(r => r.AwbNo == awbsoa);
                    if (removePrepaidDetails != null)
                        listpaymentSummaryDetails.Remove(removePrepaidDetails);
                }

                #endregion

                #region Freight Collect
                for (int j = 0; j < gridFreightCollect.Rows.Count; j++)
                {
                    gridFreightCollect.Rows[j].Cells["Validate"].Value = false;
                    clientId = Guid.Parse(gridFreightCollect.Rows[j].Cells["ClientId"].Value.ToString());
                    awbsoa = gridFreightCollect.Rows[j].Cells["AWB No"].Value.ToString();
                    paymentType = gridFreightCollect.Rows[j].Cells["Payment Type"].Value.ToString();
                    amountPaid = Convert.ToDecimal(gridFreightCollect.Rows[j].Cells["Amount Paid"].Value);
                    //if (paymentType.Equals("Cash"))
                    //{
                    //    totalCashReceived -= amountPaid;
                    //}
                    //else
                    //{
                    //    totalCheckReceived -= amountPaid;
                    //}
                    var itemRemoveFreight = listPaymentSummary.Find(r => r.ClientId == clientId);
                    if (itemRemoveFreight != null)
                        listPaymentSummary.Remove(itemRemoveFreight);

                    var removeFCDetails = listpaymentSummaryDetails.Find(r => r.AwbNo == awbsoa);
                    if (removeFCDetails != null)
                        listpaymentSummaryDetails.Remove(removeFCDetails);
                }
                #endregion

                clearPaymentSummaryData();
            }

            totalAmountReceived = totalCashReceived + totalCheckReceived;
            txtTotalCashReceived.Text = totalCashReceived.ToString();
            txtTotalCheckReceived.Text = totalCheckReceived.ToString();
            txtTotalAmntReceived.Text = totalAmountReceived.ToString();
        }

        private void lstRevenueUnitType_Enter(object sender, EventArgs e)
        {
            autoComp_revenueUnitType = new AutoCompleteStringCollection();
            var revenueUnittype = paymentSummary_revenueUnitType.OrderBy(x => x.RevenueUnitTypeName).Where(x => x.RecordStatus == 1).Select(x => x.RevenueUnitTypeName).ToList();
            foreach (var item in revenueUnittype)
            {
                autoComp_revenueUnitType.Add(item);
            }
            lstOriginBco.AutoCompleteDataSource = autoComp_revenueUnitType;
        }

        private void lstRevenueUnitName_Enter(object sender, EventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex >= 0)
            {
                var rUnitType = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                List<string> _unitName = paymentSummary_revenueUnits.Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RevenueUnitType.RevenueUnitTypeId == rUnitType).Select(x => x.RevenueUnitName).ToList();
                autoComprevenueUnitName = new AutoCompleteStringCollection();
                foreach (var item in _unitName)
                {
                    autoComprevenueUnitName.Add(item);
                }
                lstRevenueUnitName.AutoCompleteDataSource = autoComprevenueUnitName;
            }
        }

        private void lstUser_Enter(object sender, EventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex >= 0 && lstRevenueUnitName.SelectedIndex >= 0)
            {
                var rUnitType = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                var rUnitname = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());
                List<string> _empName = paymentSummary_employee.Where(x => x.AssignedToArea.City.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.AssignedToArea.RevenueUnitId == rUnitname).Select(x => x.FullName).ToList();
                autoComp_empName = new AutoCompleteStringCollection();
                foreach (var item in _empName)
                {
                    autoComp_empName.Add(item);
                }
                lstUser.AutoCompleteDataSource = autoComp_empName;
            }
        }

        private void lstRemittedBy_Enter(object sender, EventArgs e)
        {
            if (lstRevenueUnitType.SelectedIndex >= 0 && lstRevenueUnitName.SelectedIndex >= 0)
            {
                var rUnitType = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                var rUnitname = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());
                List<string> _empName = paymentSummary_employee.Where(x => x.AssignedToArea.City.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.AssignedToArea.RevenueUnitId == rUnitname && x.AssignedToArea.RevenueUnitType.RevenueUnitTypeId == rUnitType).Select(x => x.FullName).ToList();
                autoComp_empName = new AutoCompleteStringCollection();
                foreach (var item in _empName)
                {
                    autoComp_empName.Add(item);
                }
                lstRemittedBy.AutoCompleteDataSource = autoComp_empName;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Main
        private void Login()
        {
            Login loginForm = new Login();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                string username = loginForm.username;
                string password = loginForm.password;

                _userManager = new UserManager<IdentityUser, Guid>(new UserStore(GlobalVars.UnitOfWork));

                User user = userService.GetUserByUsername(username);

                if (user != null)
                {


                    if (password == Tools.Encryption.DecryptPassword(user.PasswordHash))
                    {
                        var roles = _userManager.GetRolesAsync(user.UserId).Result.ToList();
                        AppUser.Employee = user.Employee;
                        AppUser.Principal = new GenericPrincipal(new GenericIdentity(user.UserName), roles.ToArray());
                        AppUser.User = user;


                        if (user.Employee.AssignedToArea.City.BranchCorpOfficeId != GlobalVars.DeviceBcoId)
                        {
                            if (AppUser.User.UserName != "admin")
                            {
                                InvalidLogin();
                                return;
                            }
                        }

                        UserTxt.Text = "Welcome! " + AppUser.Employee.FullName;
                        btnLogOut.Enabled = true;
                    }
                    else
                    {
                        InvalidLogin();
                    }

                }
                else
                {
                    InvalidLogin();
                }
            }

            if (AppUser.Principal != null)
            {
                if (AppUser.Principal.IsInRole("Admin"))
                {
                    //btnAppSetting.Enabled = true;
                }
            }

        }

        private void InvalidLogin()
        {
            if (MessageBox.Show("Invalid username and/or password. Try again?", "APCargo", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Login();
            }
            else
            {
                this.Close();
            }
        }
        private bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                List<System.Security.Claims.Claim> list = new List<System.Security.Claims.Claim>(principal.UserClaims);
                System.Security.Claims.Claim c = list.Find(p => p.Value.Equals("S-1-5-32-544"));
                if (c != null)
                    return true;
            }
            return false;
        }

        private NewPasswordViewModel GetNewPassword()
        {
            ChangePassword changePasswordForm = new ChangePassword();
            if (changePasswordForm.ShowDialog() == DialogResult.OK)
            {
                NewPasswordViewModel vm = new NewPasswordViewModel();
                vm.OldPassword = changePasswordForm.oldPassword;
                vm.NewPassword1 = changePasswordForm.newPassword1;
                vm.NewPassword2 = changePasswordForm.newPassword2;
                return vm;
            }
            else
            { return null; }
        }


        #endregion

        #region Booking

        private void LoadInit()
        {
            bsBookingStatus = new BindingSource();
            bsBookingRemark = new BindingSource();
            bsAreas = new BindingSource();
            bsOriginBco = new BindingSource();
            bsDestinationBco = new BindingSource();

            bookingStatus = new List<BookingStatus>();
            bookingRemarks = new List<BookingRemark>();
            branchCorpOffices = new List<BranchCorpOffice>();
            areas = new List<RevenueUnit>();
            clients = new List<Entities.Client>();
            revenueUnits = new List<RevenueUnit>();
            cities = new List<City>();
            companies = new List<Company>();

            bookingStatusService = new BookingStatusBL(GlobalVars.UnitOfWork);
            bookingRemarkService = new BookingRemarkBL(GlobalVars.UnitOfWork);
            bcoService = new BranchCorpOfficeBL(GlobalVars.UnitOfWork);
            areaService = new AreaBL(GlobalVars.UnitOfWork);
            bookingService = new BookingBL(GlobalVars.UnitOfWork);
            clientService = new ClientBL(GlobalVars.UnitOfWork);
            revenueUnitService = new RevenueUnitBL(GlobalVars.UnitOfWork);
            cityService = new CityBL(GlobalVars.UnitOfWork);
            companyService = new CompanyBL(GlobalVars.UnitOfWork);
            userService = new UserStore(GlobalVars.UnitOfWork);


            bookingStatus = bookingStatusService.FilterActive().OrderBy(x => x.BookingStatusName).ToList();
            bookingRemarks = bookingRemarkService.FilterActive().OrderBy(x => x.BookingRemarkName).ToList();
            branchCorpOffices = bcoService.FilterActive().OrderBy(x => x.BranchCorpOfficeName).ToList();
            areas = areaService.FilterActive().OrderBy(x => x.RevenueUnitName).ToList();
            clients = clientService.FilterActive();
            revenueUnits = revenueUnitService.FilterActive();
            cities = cityService.FilterActive().OrderBy(x => x.CityName).ToList();
            companies = companyService.FilterActive().OrderBy(x => x.CompanyName).ToList();

            bsBookingStatus.DataSource = bookingStatus;
            bsBookingRemark.DataSource = bookingRemarks;
            bsAreas.DataSource = areas;
            bsOriginBco.DataSource = branchCorpOffices;
            bsDestinationBco.DataSource = branchCorpOffices;

            lstBookingStatus.DataSource = bsBookingStatus;
            lstBookingStatus.ValueMember = "BookingStatusId";
            lstBookingStatus.DisplayMember = "BookingStatusName";
            lstBookingStatus.SelectedIndex = -1;

            lstBookingRemarks.DataSource = bsBookingRemark;
            lstBookingRemarks.DisplayMember = "BookingRemarkName";
            lstBookingRemarks.ValueMember = "BookingRemarkId";
            lstBookingRemarks.SelectedIndex = -1;

            lstAssignedTo.DataSource = bsAreas;
            lstAssignedTo.DisplayMember = "RevenueUnitName";
            lstAssignedTo.ValueMember = "RevenueUnitId";

            lstOriginBco.DataSource = bsOriginBco;
            lstOriginBco.DisplayMember = "BranchCorpOfficeName";
            lstOriginBco.ValueMember = "BranchCorpOfficeId";
            lstOriginBco.SelectedIndex = -1;

            lstDestinationBco.DataSource = bsDestinationBco;
            lstDestinationBco.DisplayMember = "BranchCorpOfficeName";
            lstDestinationBco.ValueMember = "BranchCorpOfficeId";
            lstDestinationBco.SelectedIndex = -1;

        }

        private void NewShipment()
        {
            ShipmentModel newShipment = new ShipmentModel();
            newShipment.ShipperId = booking.ShipperId;
            newShipment.Shipper = booking.Shipper;
            newShipment.OriginCityId = booking.OriginCityId;
            newShipment.OriginCity = booking.OriginCity;
            newShipment.OriginAddress = booking.OriginAddress1;
            if (!String.IsNullOrEmpty(booking.OriginAddress2))
                newShipment.OriginAddress = newShipment.OriginAddress + ", " + booking.OriginAddress2;
            if (!String.IsNullOrEmpty(booking.OriginStreet))
                newShipment.OriginAddress = newShipment.OriginAddress + ", " + booking.OriginStreet;
            newShipment.OriginBarangay = booking.OriginBarangay;
            if (!String.IsNullOrEmpty(booking.OriginBarangay))
                newShipment.OriginAddress = newShipment.OriginAddress + ", " + booking.OriginBarangay;
            newShipment.ConsigneeId = booking.ConsigneeId;
            newShipment.Consignee = booking.Consignee;
            newShipment.DestinationCityId = booking.DestinationCityId;
            newShipment.DestinationCity = booking.DestinationCity;
            newShipment.DestinationAddress = booking.DestinationAddress1;
            if (!String.IsNullOrEmpty(booking.DestinationAddress2))
                newShipment.DestinationAddress = newShipment.DestinationAddress + ", " + booking.DestinationAddress2;
            if (!String.IsNullOrEmpty(booking.DestinationStreet))
                newShipment.DestinationAddress = newShipment.DestinationAddress + ", " + booking.DestinationStreet;
            newShipment.DestinationBarangay = booking.DestinationBarangay;
            if (!String.IsNullOrEmpty(booking.DestinationBarangay))
                newShipment.DestinationAddress = newShipment.DestinationAddress + ", " + booking.DestinationBarangay;

            newShipment.BookingId = booking.BookingId;
            newShipment.Booking = booking;


            isNewShipment = true;
            shipmentModel = newShipment;
            ((RadPageView)BookingPage.Parent).SelectedPage = this.AcceptancePage;


            BookingResetAll();
        }

        private void AddDailyBooking()
        {
            DateTime today = DateTime.Now;
            DateTime yesterday = DateTime.Now.AddDays(-1);
            List<Booking> todayBooking = bookingService.FilterActiveBy(x => x.DateBooked.Year == today.Year && x.DateBooked.Month == today.Month && x.DateBooked.Day == today.Day).ToList();
            List<Booking> yesterdayBooking = bookingService.FilterActiveBy(x => x.HasDailyBooking == true && x.DateBooked.Year == yesterday.Year && x.DateBooked.Month == yesterday.Month && x.DateBooked.Day == yesterday.Day).ToList();
            foreach (var item in yesterdayBooking)
            {
                if (!todayBooking.Exists(x => x.PreviousBookingId == item.BookingId))
                {
                    Booking newBooking = new Booking();
                    newBooking.BookingId = Guid.NewGuid();
                    newBooking.BookingNo = GetBookingNumber();
                    newBooking.DateBooked = DateTime.Now;
                    newBooking.ShipperId = item.ShipperId;
                    newBooking.OriginAddress1 = item.OriginAddress1;
                    newBooking.OriginAddress2 = item.OriginAddress2;
                    newBooking.OriginBarangay = item.OriginBarangay;
                    newBooking.OriginCityId = item.OriginCityId;
                    newBooking.ConsigneeId = item.ConsigneeId;
                    newBooking.DestinationAddress1 = item.DestinationAddress1;
                    newBooking.DestinationAddress2 = item.DestinationAddress2;
                    newBooking.DestinationBarangay = item.DestinationBarangay;
                    newBooking.DestinationCityId = item.DestinationCityId;
                    newBooking.Remarks = item.Remarks;
                    newBooking.HasDailyBooking = item.HasDailyBooking;
                    newBooking.BookedById = item.BookedById;
                    newBooking.AssignedToAreaId = item.AssignedToAreaId;
                    newBooking.BookingStatusId =
                        bookingStatus.Where(x => x.BookingStatusName.Equals("Pending"))
                            .First()
                            .BookingStatusId;
                    newBooking.BookingRemarkId =
                        bookingRemarks.Where(x => x.BookingRemarkName.Equals("Lack of Time"))
                            .First()
                            .BookingRemarkId;
                    newBooking.CreatedBy = item.CreatedBy;
                    newBooking.CreatedDate = DateTime.Now;
                    newBooking.ModifiedBy = item.CreatedBy;
                    newBooking.ModifiedDate = DateTime.Now;
                    newBooking.RecordStatus = (int)RecordStatus.Active;
                    newBooking.PreviousBookingId = item.BookingId;
                    bookingService.AddEdit(newBooking);
                }
            }
        }

        private void PopulateGrid(List<Booking> bookings = null)
        {
            List<Booking> _bookings;

            if (bookings == null)
            {
                _bookings = bookingService.GetAll().Where(x => x.RecordStatus == 1).OrderBy(x => x.DateBooked).OrderByDescending(x => x.CreatedDate)
                        .ToList();
            }
            else
            {
                _bookings = bookings;
            }

            BookingGridView.DataSource = ConvertToDataTable(_bookings);

            BookingGridView.Columns["BookingId"].IsVisible = false;
            BookingGridView.Columns["Booking Date"].Width = 100;
            BookingGridView.Columns["Booking Number"].Width = 150;
            BookingGridView.Columns["Booking Time"].Width = 100;
            BookingGridView.Columns["Shipper Name"].Width = 150;
            BookingGridView.Columns["Origin City"].Width = 150;
            BookingGridView.Columns["Consignee Name"].Width = 150;
            BookingGridView.Columns["Destination City"].Width = 150;
            BookingGridView.Columns["Booked By"].Width = 150;
            BookingGridView.Columns["Booking Status"].Width = 100;

        }

        private DataTable ConvertToDataTable(List<Booking> list)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("BookingId", typeof(string)));
            dt.Columns.Add(new DataColumn("Booking Date", typeof(string)));
            dt.Columns.Add(new DataColumn("Booking Time", typeof(string)));
            dt.Columns.Add(new DataColumn("Booking Number", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Origin City", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Destination City", typeof(string)));
            dt.Columns.Add(new DataColumn("Booked By", typeof(string)));
            dt.Columns.Add(new DataColumn("Booking Status", typeof(string)));

            dt.BeginLoadData();

            foreach (Booking item in list)
            {
                DataRow row = dt.NewRow();
                row["BookingId"] = item.BookingId.ToString();
                row["Booking Date"] = item.DateBooked.ToString("MMM dd, yyyy");
                row["Booking Time"] = item.DateBooked.ToString("hh:mm tt");
                row["Booking Number"] = item.BookingNo.ToString();
                row["Shipper Name"] = item.Shipper.FullName;
                row["Origin City"] = item.OriginCity.CityName;
                row["Consignee Name"] = item.Consignee.FullName;
                row["Destination City"] = item.DestinationCity.CityName;
                row["Booked By"] = item.BookedBy.FullName;
                row["Booking Status"] = item.BookingStatus.BookingStatusName.ToString();

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            return dt;
        }

        private DataTable ConvertToDataTable(List<ShipmentModel> list)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(int)));
            dt.Columns.Add(new DataColumn("Awb No", typeof(string)));
            dt.Columns.Add(new DataColumn("Date Accepted", typeof(string)));
            dt.Columns.Add(new DataColumn("Account No", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Shipper Address", typeof(string)));
            dt.Columns.Add(new DataColumn("Origin BCO", typeof(string)));
            dt.Columns.Add(new DataColumn("Origin City", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Consignee Address", typeof(string)));
            dt.Columns.Add(new DataColumn("Destination BCO", typeof(string)));
            dt.Columns.Add(new DataColumn("Destination City", typeof(string)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(string)));
            dt.Columns.Add(new DataColumn("Chargeable Weight", typeof(string)));
            dt.Columns.Add(new DataColumn("Sub Total", typeof(string)));
            dt.Columns.Add(new DataColumn("Vat Amount", typeof(string)));
            dt.Columns.Add(new DataColumn("Total", typeof(string)));
            dt.Columns.Add(new DataColumn("Service Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Payment Mode", typeof(string)));
            dt.Columns.Add(new DataColumn("Accepted By", typeof(string)));
            dt.Columns.Add(new DataColumn("Booking Assignment", typeof(string)));
            dt.Columns.Add(new DataColumn("User Assignment", typeof(string)));
            dt.Columns.Add(new DataColumn("Prepared By", typeof(string)));

            dt.BeginLoadData();

            int index = 1;
            foreach (ShipmentModel item in list)
            {

                DataRow row = dt.NewRow();
                row["No"] = index;
                row["Awb No"] = item.AirwayBillNo;
                row["Date Accepted"] = item.DateAcceptedString;
                row["Account No"] = item.Shipper.AccountNo;
                row["Shipper Name"] = item.Shipper.FullName;
                row["Shipper Address"] = item.OriginAddress;
                row["Origin BCO"] = item.OriginCity.BranchCorpOffice.BranchCorpOfficeName;
                row["Origin City"] = item.OriginCity.CityName;
                row["Consignee Name"] = item.Consignee.FullName;
                row["Consignee Address"] = item.DestinationAddress;
                row["Destination BCO"] = item.DestinationCity.BranchCorpOffice.BranchCorpOfficeName;
                row["Destination City"] = item.DestinationCity.CityName;
                row["Quantity"] = item.Quantity.ToString();
                row["Chargeable Weight"] = item.ChargeableWeightString;
                row["Sub Total"] = item.ShipmentSubTotalString;
                row["Vat Amount"] = item.ShipmentVatAmountString;
                row["Total"] = item.ShipmentTotalString;
                row["Service Mode"] = item.ServiceMode.ServiceModeName;
                row["Payment Mode"] = item.PaymentMode.PaymentModeCode;
                row["Accepted By"] = item.AcceptedBy.FullName;
                row["Booking Assignment"] = item.Booking.AssignedToArea != null ? item.Booking.AssignedToArea.RevenueUnitName : "N/A";
                User preparedBy = GetPreparedBy(item.Booking.CreatedBy);
                row["User Assignment"] = preparedBy.Employee.AssignedToArea.RevenueUnitName;
                row["Prepared By"] = preparedBy.UserName;
                index++;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="Evm"></param>
        /// <returns></returns>
        private DataTable ConvertToDataTable(ShipmentModel list, out decimal Evm)
        {
            decimal totalEvm = 0;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Length", typeof(string)));
            dt.Columns.Add(new DataColumn("Width", typeof(string)));
            dt.Columns.Add(new DataColumn("Height", typeof(string)));
            dt.Columns.Add(new DataColumn("Crating", typeof(string)));
            dt.Columns.Add(new DataColumn("Packaging", typeof(string)));
            dt.Columns.Add(new DataColumn("Draining", typeof(string)));

            dt.BeginLoadData();

            foreach (PackageDimensionModel item in list.PackageDimensions)
            {
                totalEvm = totalEvm + item.Evm;
                DataRow row = dt.NewRow();
                row["No"] = item.Index + 1;
                row["Length"] = item.LengthString;
                row["Width"] = item.WidthString;
                row["Height"] = item.HeightString;
                row["Crating"] = item.CratingFeeString;
                row["Packaging"] = item.PackagingFeeString;
                row["Draining"] = item.DrainingFeeString;

                dt.Rows.Add(row);
            }

            dt.EndLoadData();
            Evm = totalEvm;
            return dt;
        }

        private User GetPreparedBy(Guid id)
        {
            return userService.GetUserById(id);
        }

        private void BookingSelected(Guid id)
        {
            booking = bookingService.FilterActiveBy(x => x.BookingId == id).FirstOrDefault();

            btnNew.Enabled = true;
            btnSave.Enabled = false;
            btnReset.Enabled = true;
            btnAcceptance.Enabled = true;
            btnDelete.Enabled = true;
            btnEdit.Enabled = true;

            GroupShipper.Enabled = false;
            GroupConsignee.Enabled = false;
            GroupRemarks.Enabled = false;

            BookingPopulateForm();
        }

        private void BookingPopulateForm()
        {
            if (booking != null)
            {
                txtShipperAccountNo.Text = booking.Shipper.AccountNo;
                txtShipperLastName.Text = booking.Shipper.LastName;
                txtShipperFirstName.Text = booking.Shipper.FirstName;
                if (booking.Shipper.CompanyId != null)
                {
                    txtShipperCompany.Text = booking.Shipper.Company.CompanyName + " - " + booking.Shipper.Company.AccountNo;
                }
                else
                {
                    txtShipperCompany.Text = "N/A";
                }
                txtShipperAddress1.Text = booking.OriginAddress1;
                txtShipperAddress2.Text = booking.OriginAddress2;
                txtShipperStreet.Text = booking.OriginStreet;
                txtShipperBarangay.Text = booking.OriginBarangay;
                var bcoId = cities.FirstOrDefault(x => x.CityId == booking.OriginCityId).BranchCorpOffice.BranchCorpOfficeId;
                SelectedOriginCity(bcoId);
                lstOriginBco.SelectedValue = bcoId;
                lstOriginCity.SelectedValue = booking.OriginCityId;
                txtShipperContactNo.Text = booking.Shipper.ContactNo;
                txtShipperMobile.Text = booking.Shipper.Mobile;
                txtShipperEmail.Text = booking.Shipper.Email;

                txtConsigneeAccountNo.Text = booking.Consignee.AccountNo;
                txtConsigneeLastName.Text = booking.Consignee.LastName;
                txtConsigneeFirstName.Text = booking.Consignee.FirstName;
                if (booking.Consignee.CompanyId != null)
                {
                    txtConsigneeCompany.Text = booking.Consignee.Company.CompanyName + " - " + booking.Consignee.Company.AccountNo;
                }
                else
                {
                    txtConsigneeCompany.Text = "N/A";
                }
                txtConsigneeAddress1.Text = booking.DestinationAddress1;
                txtConsigneeAddress2.Text = booking.DestinationAddress2;
                txtConsgineeStreet.Text = booking.DestinationStreet;
                txtConsigneeBarangay.Text = booking.DestinationBarangay;
                bcoId = cities.FirstOrDefault(x => x.CityId == booking.DestinationCityId).BranchCorpOffice.BranchCorpOfficeId;
                SelectedDestinationCity(bcoId);
                lstDestinationBco.SelectedValue = bcoId;
                lstDestinationCity.SelectedValue = booking.DestinationCityId;
                txtConsigneeContactNo.Text = booking.Consignee.ContactNo;
                txtConsigneeMobile.Text = booking.Consignee.Mobile;
                txtConsigneeEmail.Text = booking.Consignee.Email;

                dateDateBooked.Value = booking.DateBooked;
                txtBookedBy.Text = booking.BookedBy.FullName;
                txtRemarks.Text = booking.Remarks;
                txtBookingNo.Text = booking.BookingNo;
                chkHasDailyBooking.Checked = booking.HasDailyBooking;
                lstAssignedTo.SelectedValue = booking.AssignedToAreaId;
                lstBookingStatus.SelectedValue = booking.BookingStatusId;
                lstBookingRemarks.SelectedIndex = -1;
                if (booking.BookingRemarkId != null)
                {
                    lstBookingRemarks.SelectedValue = booking.BookingRemarkId;
                }

                shipper = clientService.GetById(booking.ShipperId);
                consignee = clientService.GetById(booking.ConsigneeId);

            }
        }

        private void NewBooking()
        {

            if (AppUser.Principal.Identity.IsAuthenticated)
            {

                string bookingNo = GetBookingNumber();
                txtBookingNo.Text = bookingNo;
                booking = new Booking();
                booking.BookingNo = bookingNo;
                booking.BookedById = AppUser.User.EmployeeId;

                txtShipperAccountNo.Text = "";
                txtShipperLastName.Text = "";
                txtShipperFirstName.Text = "";
                txtShipperCompany.Text = "";
                txtShipperAddress1.Text = "";
                txtShipperAddress2.Text = "N/A";
                txtShipperStreet.Text = "N/A";
                txtShipperBarangay.Text = "N/A";
                if (lstOriginBco.Items.Count > 0)
                    lstOriginBco.SelectedIndex = -1;
                if (lstOriginCity.Items.Count > 0)
                    lstOriginCity.SelectedIndex = -1;
                txtShipperContactNo.Text = "0000000";
                txtShipperMobile.Text = "00000000000";
                txtShipperEmail.Text = "N/A";

                txtConsigneeAccountNo.Text = "";
                txtConsigneeLastName.Text = "";
                txtConsigneeFirstName.Text = "";
                txtConsigneeCompany.Text = "";
                txtConsigneeAddress1.Text = "";
                txtConsigneeAddress2.Text = "N/A";
                txtConsgineeStreet.Text = "N/A";
                txtConsigneeBarangay.Text = "N/A";
                if (lstDestinationBco.Items.Count > 0)
                    lstDestinationBco.SelectedIndex = -1;
                if (lstDestinationCity.Items.Count > 0)
                    lstDestinationCity.SelectedIndex = -1;
                txtConsigneeContactNo.Text = "0000000";
                txtConsigneeMobile.Text = "00000000000";
                txtConsigneeEmail.Text = "N/A";

                txtRemarks.Text = "";
                dateDateBooked.Value = DateTime.Now;
                // chkHasDailyBooking.Checked = false;
                txtBookedBy.Text = "";

                if (lstAssignedTo.Items.Count > 0)
                    lstAssignedTo.SelectedIndex = 0;

                if (lstBookingStatus.Items.Count > 0)
                {
                    lstBookingStatus.SelectedIndex = 1;
                }
                if (lstBookingRemarks.Items.Count > 0)
                    lstBookingRemarks.SelectedIndex = -1;

                //txtShipperLastName.Enabled = true;
                //txtShipperFirstName.Enabled = true;
                //txtShipperCompany.Enabled = true;
                //txtShipperAddress1.Enabled = true;
                //txtShipperAddress2.Enabled = true;
                //txtShipperStreet.Enabled = true;
                //txtShipperBarangay.Enabled = true;
                //lstOriginBco.Enabled = true;
                //lstOriginCity.Enabled = true;
                //txtShipperContactNo.Enabled = true;
                //txtShipperMobile.Enabled = true;
                //txtShipperEmail.Enabled = true;

                //txtConsigneeLastName.Enabled = true;
                //txtConsigneeFirstName.Enabled = true;
                //txtConsigneeCompany.Enabled = true;
                //txtConsigneeAddress1.Enabled = true;
                //txtConsigneeAddress2.Enabled = true;
                //txtConsgineeStreet.Enabled = true;
                //txtConsigneeBarangay.Enabled = true;
                //lstDestinationBco.Enabled = true;
                //lstDestinationCity.Enabled = true;
                //txtConsigneeContactNo.Enabled = true;
                //txtConsigneeMobile.Enabled = true;
                //txtConsigneeEmail.Enabled = true;

                GroupShipper.Enabled = true;
                GroupConsignee.Enabled = true;
                GroupRemarks.Enabled = true;

                //txtRemarks.Enabled = true;
                //dateDateBooked.Enabled = true;
                //chkHasDailyBooking.Enabled = true;
                //txtBookedBy.Enabled = true;
                //txtBookingNo.Enabled = true;
                //lstAssignedTo.Enabled = true;
                //lstBookingStatus.Enabled = false;
                //lstBookingRemarks.Enabled = false;

                btnNew.Enabled = false;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                btnReset.Enabled = true;
                btnAcceptance.Enabled = false;
                btnDelete.Enabled = false;

                lstOriginBco.SelectedValue = GlobalVars.DeviceBcoId;
                txtShipperLastName.Focus();
                this.ActiveControl = txtShipperLastName;
            }
        }

        private void DeleteBooking()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                booking.ModifiedBy = AppUser.User.UserId;
                booking.ModifiedDate = DateTime.Now;
                booking.RecordStatus = (int)RecordStatus.Deleted;
                bookingService.AddEdit(booking);
                AcceptanceResetAll();
                PopulateGrid();
            }
        }

        private void BookingResetAll()
        {
            txtShipperAccountNo.Text = "";
            txtShipperLastName.Text = "";
            txtShipperFirstName.Text = "";
            txtShipperCompany.Text = "";
            txtShipperAddress1.Text = "";
            txtShipperAddress2.Text = "";
            txtShipperStreet.Text = "";
            txtShipperBarangay.Text = "";
            if (lstOriginBco.Items.Count > 0)
                lstOriginBco.SelectedIndex = -1;
            if (lstOriginCity.Items.Count > 0)
                lstOriginCity.SelectedIndex = -1;
            txtShipperContactNo.Text = txtShipperContactNo.Mask;
            txtShipperMobile.Text = txtShipperMobile.Mask;
            txtShipperEmail.Text = "";

            txtConsigneeAccountNo.Text = "";
            txtConsigneeLastName.Text = "";
            txtConsigneeFirstName.Text = "";
            txtConsigneeCompany.Text = "";
            txtConsigneeAddress1.Text = "";
            txtConsigneeAddress2.Text = "";
            txtConsgineeStreet.Text = "";
            txtConsigneeBarangay.Text = "";
            if (lstDestinationBco.Items.Count > 0)
                lstDestinationBco.SelectedIndex = -1;
            if (lstDestinationCity.Items.Count > 0)
                lstDestinationCity.SelectedIndex = -1;
            txtConsigneeContactNo.Text = txtConsigneeContactNo.Mask;
            txtConsigneeMobile.Text = txtConsigneeMobile.Mask;
            txtConsigneeEmail.Text = "";

            txtRemarks.Text = "";

            //chkHasDailyBooking.Checked = false;
            txtBookedBy.Text = "";
            txtBookingNo.Text = "";
            if (lstAssignedTo.Items.Count > 0)
                lstAssignedTo.SelectedIndex = 0;
            if (lstBookingStatus.Items.Count > 0)
                lstBookingStatus.SelectedIndex = 0;
            if (lstBookingRemarks.Items.Count > 0)
                lstBookingRemarks.SelectedIndex = -1;


            GroupShipper.Enabled = false;
            GroupConsignee.Enabled = false;
            GroupRemarks.Enabled = false;


            //txtRemarks.Enabled = false;
            //dateDateBooked.Enabled = false;
            //chkHasDailyBooking.Enabled = false;
            //txtBookedBy.Enabled = false;
            //txtBookingNo.Enabled = false;
            //lstAssignedTo.Enabled = false;
            //lstBookingStatus.Enabled = false;
            //lstBookingRemarks.Enabled = false;

            btnNew.Enabled = true;
            btnSave.Enabled = false;
            btnReset.Enabled = false;
            btnAcceptance.Enabled = false;
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
        }

        private string GetBookingNumber()
        {
            string date = DateTime.Now.ToString("yy");
            int lastBooking = 1;
            var deviceCode = ConfigurationSettings.AppSettings["DeviceCode"];
            var bookings = bookingService.FilterActive();
            if (bookings != null && bookings.Count > 0)
            {
                lastBooking = Convert.ToInt32(bookings.Max(x => Convert.ToInt32(x.BookingNo.Substring(x.BookingNo.Length - 5, 5)))) + 1;
            }
            if (string.IsNullOrEmpty(deviceCode))
            {
                deviceCode = "C000";
            }

            return deviceCode + date + lastBooking.ToString("00000");
        }

        private Guid GetCompanyIdByString(string companyname)
        {
            int startindex = companyname.IndexOf("-") + 2;
            string acctNo = companyname.Substring(startindex, companyname.Length - startindex);
            var company = companies.First(x => x.AccountNo.Equals(acctNo));
            if (company != null)
                return company.CompanyId;
            else
                return new Guid();
        }

        private void CreateShipper()
        {
            Entities.Client _client = clients.FirstOrDefault(x => x.LastName.Equals(txtShipperLastName.Text.Trim()) && x.FirstName.Equals(txtShipperFirstName.Text.Trim()));
            if (_client != null)
            {
                shipper = _client;
                booking.ShipperId = shipper.ClientId;
                booking.Shipper = shipper;
                txtShipperAccountNo.Text = shipper.AccountNo;
                if (shipper.CompanyId != null)
                { txtShipperCompany.Text = shipper.Company.CompanyName + " - " + shipper.Company.AccountNo; }
                else
                {
                    txtShipperCompany.Text = shipper.Company.CompanyName;
                }
                txtShipperAddress1.Text = shipper.Address1;
                txtShipperAddress2.Text = shipper.Address2;
                txtShipperStreet.Text = shipper.Street;
                txtShipperBarangay.Text = shipper.Barangay;
                lstOriginBco.Text = shipper.City.BranchCorpOffice.BranchCorpOfficeName;
                lstOriginCity.Text = shipper.City.CityName;
                if (shipper.City != null)
                {
                    lstOriginBco.SelectedValue = shipper.City.BranchCorpOfficeId;
                    lstOriginCity.SelectedValue = shipper.City.CityId;
                }
                txtShipperContactNo.Text = shipper.ContactNo;
                txtShipperMobile.Text = shipper.Mobile;
                txtShipperEmail.Text = shipper.Email;
            }
            else
            {
                shipper = new Entities.Client();
                shipper.LastName = txtShipperLastName.Text.Trim();
                shipper.FirstName = txtShipperFirstName.Text.Trim();
            }
        }

        private void CreateConsignee()
        {
            var _client = clients.FirstOrDefault(x => x.LastName.Equals(txtConsigneeLastName.Text.Trim()) && x.FirstName.Equals(txtConsigneeFirstName.Text.Trim()));
            if (_client != null)
            {
                consignee = _client;
                booking.ConsigneeId = consignee.ClientId;
                booking.Consignee = consignee;
                txtConsigneeAccountNo.Text = consignee.AccountNo;
                if (consignee.CompanyId != null)
                { txtConsigneeCompany.Text = consignee.Company.CompanyName + " - " + consignee.Company.AccountNo; }
                else
                { txtConsigneeCompany.Text = consignee.Company.CompanyName; }

                txtConsigneeAddress1.Text = consignee.Address1;
                txtConsigneeAddress2.Text = consignee.Address2;
                txtConsgineeStreet.Text = consignee.Street;
                txtConsigneeBarangay.Text = consignee.Barangay;
                lstDestinationBco.Text = consignee.City.BranchCorpOffice.BranchCorpOfficeName;
                lstDestinationCity.Text = consignee.City.CityName;
                if (consignee.City != null)
                {
                    lstDestinationBco.SelectedValue = consignee.City.BranchCorpOfficeId;
                    lstDestinationCity.SelectedValue = consignee.City.CityId;
                }
                txtConsigneeContactNo.Text = consignee.ContactNo;
                txtConsigneeMobile.Text = consignee.Mobile;
                txtConsigneeEmail.Text = consignee.Email;
            }
            else
            {
                consignee = new Entities.Client();
                consignee.LastName = txtConsigneeLastName.Text.Trim();
                consignee.FirstName = txtConsigneeFirstName.Text.Trim();
            }
            btnSave.Enabled = true;
        }

        private bool IsDataValid()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(txtShipperLastName.Text))
            {
                MessageBox.Show("Invalid Shipper Lastname", "Data Error", MessageBoxButtons.OK);
                txtShipperLastName.Focus();
                isValid = false;
                return isValid;
            }
            if (string.IsNullOrEmpty(txtShipperFirstName.Text))
            {
                MessageBox.Show("Invalid Shipper Firstname", "Data Error", MessageBoxButtons.OK);
                txtShipperFirstName.Focus();
                isValid = false;
                return isValid;
            }
            if (string.IsNullOrEmpty(txtShipperAddress1.Text))
            {
                MessageBox.Show("Invalid Shipper Address", "Data Error", MessageBoxButtons.OK);
                txtShipperAddress1.Focus();
                isValid = false;
                return isValid;
            }
            if (string.IsNullOrEmpty(txtConsigneeLastName.Text))
            {
                MessageBox.Show("Invalid Consignee Lastname", "Data Error", MessageBoxButtons.OK);
                txtConsigneeLastName.Focus();
                isValid = false;
                return isValid;
            }
            if (string.IsNullOrEmpty(txtConsigneeFirstName.Text))
            {
                MessageBox.Show("Invalid Consignee Firstname", "Data Error", MessageBoxButtons.OK);
                txtConsigneeFirstName.Focus();
                isValid = false;
                return isValid;
            }
            if (string.IsNullOrEmpty(txtConsigneeAddress1.Text))
            {
                MessageBox.Show("Invalid Consignee Address", "Data Error", MessageBoxButtons.OK);
                txtConsigneeAddress1.Focus();
                isValid = false;
                return isValid;
            }
            //if (lstAssignedTo.SelectedIndex <= -1)
            //{
            //    MessageBox.Show("Booking is not assigned to an Area.", "Data Error", MessageBoxButtons.OK);
            //    lstAssignedTo.Focus();
            //    isValid = false;
            //    return isValid;
            //}
            if (lstBookingStatus.SelectedIndex <= -1)
            {
                MessageBox.Show("Invalid Booking Status", "Data Error", MessageBoxButtons.OK);
                lstBookingStatus.Focus();
                isValid = false;
                return isValid;
            }

            if (lstOriginBco.SelectedIndex < 0)
            {
                if (lstOriginBco.Text == "")
                {
                    MessageBox.Show("Invalid Shipper BCO.", "Data Error", MessageBoxButtons.OK);
                    isValid = false;
                    lstOriginBco.Focus();
                    return isValid;
                }

            }

            if (lstOriginCity.SelectedIndex < 0)
            {
                if (lstOriginCity.Text == "")
                {
                    MessageBox.Show("Invalid Shipper City.", "Data Error", MessageBoxButtons.OK);
                    isValid = false;
                    lstOriginCity.Focus();
                    return isValid;
                }

            }

            if (lstDestinationBco.SelectedIndex < 0)
            {
                if (lstDestinationBco.Text == "")
                {
                    MessageBox.Show("Invalid Consignee BCO.", "Data Error", MessageBoxButtons.OK);
                    isValid = false;
                    lstDestinationBco.Focus();
                    return isValid;
                }
            }

            if (lstDestinationCity.SelectedIndex < 0)
            {
                if (lstDestinationCity.Text == "")
                {
                    MessageBox.Show("Invalid Consignee City.", "Data Error", MessageBoxButtons.OK);
                    lstDestinationCity.Focus();
                    isValid = false;
                    return isValid;
                }

            }

            if ((txtShipperContactNo.Text == txtShipperContactNo.Mask) && (txtShipperMobile.Text == txtShipperMobile.Mask))
            {
                MessageBox.Show("Atleast one contact number is required.", "Data Error", MessageBoxButtons.OK);
                txtShipperContactNo.Focus();
                isValid = false;
                return isValid;
            }

            //if ((!txtShipperContactNo.) || (!txtShipperContactNo.MaskedEditBoxElement))
            //{
            //    toolTip1.ToolTipTitle = "Invalid contact number.";
            //    toolTip1.Show("Contact No must be in 000-0000 format.", txtShipperContactNo, -16, -73, 5000);
            //    txtShipperContactNo.Focus();
            //    isValid = false;
            //    return isValid;
            //}
            //if ((!txtShipperMobile.MaskFull) || (!txtShipperMobile.MaskCompleted))
            //{
            //    toolTip1.ToolTipTitle = "Invalid Shipper number.";
            //    toolTip1.Show("Mobile No must be in (000)000-0000 format.", txtShipperMobile, -16, -73, 5000);
            //    txtShipperMobile.Focus();
            //    isValid = false;
            //    return isValid;
            //}

            if ((txtConsigneeContactNo.Text == txtConsigneeContactNo.Mask) && (txtConsigneeMobile.Text == txtConsigneeMobile.Mask))
            {
                MessageBox.Show("Atleast one contact number is required.", "Data Error", MessageBoxButtons.OK);
                txtConsigneeContactNo.Focus();
                isValid = false;
                return isValid;
            }

            //if ((!txtConsigneeContactNo.MaskFull) || (!txtConsigneeContactNo.MaskCompleted))
            //{
            //    toolTip1.ToolTipTitle = "Invalid Consignee number.";
            //    toolTip1.Show("Contact No must be in 000-0000 format.", txtConsigneeContactNo, -16, -73, 5000);
            //    txtConsigneeContactNo.Focus();
            //    isValid = false;
            //    return isValid;
            //}

            //if ((!txtConsigneeMobile.MaskFull) || (!txtConsigneeMobile.MaskCompleted))
            //{
            //    toolTip1.ToolTipTitle = "Invalid Consignee number.";
            //    toolTip1.Show("Mobile No must be in (000)000-0000 format.", txtConsigneeMobile, -16, -73, 5000);
            //    txtConsigneeMobile.Focus();
            //    isValid = false;
            //    return isValid;
            //}

            //if (!isMailValid(txtShipperEmail.Text))
            //{
            //    toolTip1.ToolTipTitle = "Invalid email address.";
            //    toolTip1.Show("Email must be in correct format.", txtShipperEmail, -16, -73, 5000);
            //    txtShipperEmail.Focus();
            //    isValid = false;
            //    return isValid;
            //}

            //if (!isMailValid(txtConsigneeEmail.Text))
            //{
            //    toolTip1.ToolTipTitle = "Invalid email address.";
            //    toolTip1.Show("Email must be in correct format.", txtConsigneeEmail, -16, -73, 5000);
            //    txtConsigneeEmail.Focus();
            //    isValid = false;
            //    return isValid;
            //}

            return isValid;
        }

        private Boolean isMailValid(string emailAddress)
        {
            try
            {
                MailAddress mail = new MailAddress(emailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private Boolean IsNumericOnly(int intMin, int intMax, string strNumericOnly)
        {
            Boolean blnResult = false;
            Regex regexPattern = new Regex("^[0-9\\s]{" + intMin.ToString() + "," + intMax.ToString() + "}$");
            if ((strNumericOnly.Length >= intMin & strNumericOnly.Length <= intMax))
            {
                // check here if there are other none alphanumeric characters
                strNumericOnly = strNumericOnly.Trim();
                blnResult = regexPattern.IsMatch(strNumericOnly);
            }
            else
            {
                blnResult = false;
            }
            return blnResult;
        }

        private void SaveBooking()
        {
            if (IsDataValid())
            {
                #region ShipperInfo
                shipper.CreatedBy = AppUser.User.UserId;
                shipper.CreatedDate = DateTime.Now;
                shipper.ModifiedBy = AppUser.User.UserId;
                shipper.ModifiedDate = DateTime.Now;
                shipper.RecordStatus = (int)RecordStatus.Active;
                Company company = companies.Find(x => x.CompanyName == txtShipperCompany.Text.Trim());
                if (company != null)
                {
                    shipper.Company = company;
                    shipper.CompanyId = company.CompanyId;
                }
                shipper.Address1 = txtShipperAddress1.Text.Trim();
                shipper.Address2 = txtShipperAddress2.Text.Trim();
                shipper.Street = txtShipperStreet.Text.Trim();
                shipper.Barangay = txtShipperBarangay.Text.Trim();
                if (lstOriginCity.SelectedIndex >= 0)
                {
                    shipper.CityId = Guid.Parse(lstOriginCity.SelectedValue.ToString());
                }
                else
                {
                    MessageBox.Show("Invalid Shipper City.", "Data Error", MessageBoxButtons.OK);
                    return;
                }

                shipper.ContactNo = txtShipperContactNo.Text.Trim();
                shipper.Mobile = txtShipperMobile.Text.Trim();
                shipper.Email = txtShipperEmail.Text.Trim();

                #endregion

                #region ConsingnessInfo
                consignee.CreatedBy = AppUser.User.UserId;
                consignee.CreatedDate = DateTime.Now;
                consignee.ModifiedBy = AppUser.User.UserId;
                consignee.ModifiedDate = DateTime.Now;
                consignee.RecordStatus = (int)RecordStatus.Active;
                Company consigneeCompany = companies.Find(x => x.CompanyName == txtConsigneeCompany.Text.Trim());
                if (consigneeCompany != null)
                {
                    consignee.Company = consigneeCompany;
                    consignee.CompanyId = consigneeCompany.CompanyId;
                }
                consignee.Address1 = txtConsigneeAddress1.Text.Trim();
                consignee.Address2 = txtConsigneeAddress2.Text.Trim();
                consignee.Street = txtConsgineeStreet.Text.Trim();
                consignee.Barangay = txtConsigneeBarangay.Text.Trim();
                if (lstDestinationCity.SelectedIndex >= 0)
                {
                    consignee.CityId = Guid.Parse(lstDestinationCity.SelectedValue.ToString());
                }
                else
                {
                    MessageBox.Show("Invalid Consignee City.", "Data Error", MessageBoxButtons.OK);
                    return;
                }
                consignee.ContactNo = txtConsigneeContactNo.Text.Trim();
                consignee.Mobile = txtConsigneeMobile.Text.Trim();
                consignee.Email = txtConsigneeEmail.Text.Trim();
                if (consignee.CompanyId == null)

                #endregion

                    #region CaptureBookingInput
                    booking.OriginAddress1 = txtShipperAddress1.Text.Trim();
                booking.OriginAddress2 = txtShipperAddress2.Text.Trim();
                booking.OriginStreet = txtShipperStreet.Text.Trim();
                booking.OriginBarangay = txtShipperBarangay.Text.Trim();
                booking.OriginCityId = Guid.Parse(lstOriginCity.SelectedValue.ToString());
                booking.DestinationAddress1 = txtConsigneeAddress1.Text.Trim();
                booking.DestinationAddress2 = txtConsigneeAddress2.Text.Trim();
                booking.DestinationStreet = txtConsgineeStreet.Text.Trim();
                booking.DestinationBarangay = txtConsigneeBarangay.Text.Trim();
                booking.DestinationCityId = Guid.Parse(lstDestinationCity.SelectedValue.ToString());
                booking.DateBooked = dateDateBooked.Value;
                booking.Remarks = txtRemarks.Text;
                booking.HasDailyBooking = chkHasDailyBooking.Checked;
                if (lstAssignedTo.SelectedIndex > -1)
                {
                    booking.AssignedToAreaId = Guid.Parse(lstAssignedTo.SelectedValue.ToString());
                }

                booking.BookingStatusId = Guid.Parse(lstBookingStatus.SelectedValue.ToString());
                if (lstBookingRemarks.SelectedValue != null)
                    booking.BookingRemarkId = Guid.Parse(lstBookingRemarks.SelectedValue.ToString());
                booking.ModifiedBy = AppUser.User.UserId;
                booking.ModifiedDate = DateTime.Now;
                booking.RecordStatus = (int)RecordStatus.Active;
                if (booking.BookingId == null || booking.BookingId == Guid.Empty)
                {
                    booking.BookingId = Guid.NewGuid();
                    booking.CreatedBy = AppUser.User.UserId;
                    booking.CreatedDate = DateTime.Now;
                }
                    #endregion

                ProgressIndicator saving = new ProgressIndicator("Booking", "Saving ...", Saving);
                saving.ShowDialog();

                if (booking.AssignedToAreaId == null || booking.AssignedToAreaId == Guid.Empty)
                {
                    BookingSelected(booking.BookingId);
                    NewShipment();
                }
                else
                {
                    PopulateGrid();
                    BookingResetAll();
                }

                booking = null;
                shipper = null;
                consignee = null;
            }
        }

        private void Saving(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _worker = sender as BackgroundWorker;
            int percent = 1;
            int index = 1;
            int max = 2; // # of processes

            #region NewClient
            if (shipper.ClientId == null || shipper.ClientId == Guid.Empty)
            {
                shipper.ClientId = Guid.NewGuid();
                if (shipper.City == null)
                    shipper.City = cities.FirstOrDefault(x => x.CityId == shipper.CityId);
                if (shipper.CompanyId == null)
                {
                    // non-rep client account #
                    shipper.AccountNo = "1" + clientService.GetNewAccountNo(shipper.City.CityCode, false);
                }
                else
                {
                    shipper.AccountNo = "2" + clientService.GetNewAccountNo(shipper.City.CityCode, false);
                }
                clientService.Add(shipper);
                booking.ShipperId = shipper.ClientId;
            }

            if (consignee.ClientId == null || consignee.ClientId == Guid.Empty)
            {
                consignee.ClientId = Guid.NewGuid();
                if (consignee.City == null)
                    consignee.City = cities.FirstOrDefault(x => x.CityId == consignee.CityId);
                if (consignee.CompanyId == null)
                {
                    // non-rep client account #
                    consignee.AccountNo = "1" + clientService.GetNewAccountNo(consignee.City.CityCode, false);
                }
                else
                {
                    consignee.AccountNo = "2" + clientService.GetNewAccountNo(consignee.City.CityCode, false);
                }
                consignee.AccountNo = clientService.GetNewAccountNo(consignee.City.CityCode, false);
                clientService.Add(consignee);
                booking.ConsigneeId = consignee.ClientId;
            }
            percent = index * 100 / max;
            _worker.ReportProgress(percent);
            index++;

            #endregion

            #region SaveBooking
            bookingService.AddEdit(booking);

            percent = index * 100 / max;
            _worker.ReportProgress(percent);
            index++;
            #endregion

        }

        private void SelectedDestinationCity(Guid bcoId)
        {
            List<City> _cities = cities.Where(x => x.BranchCorpOffice.BranchCorpOfficeId == bcoId).OrderBy(x => x.CityName).ToList();
            lstDestinationCity.DataSource = _cities;
            lstDestinationCity.DisplayMember = "CityName";
            lstDestinationCity.ValueMember = "CityId";
        }

        private void SelectedOriginCity(Guid bcoId)
        {
            List<City> _cities = cities.Where(x => x.BranchCorpOffice.BranchCorpOfficeId == bcoId).OrderBy(x => x.CityName).ToList();  // x => cityIds.Contains(x.CityId)).OrderBy(x => x.CityName).ToList();
            lstOriginCity.DataSource = _cities;
            lstOriginCity.DisplayMember = "CityName";
            lstOriginCity.ValueMember = "CityId";

        }

        private bool ValidateData()
        {
            if (lstCommodityType.SelectedIndex <= -1 || lstCommodity.SelectedIndex <= -1 || lstGoodsDescription.SelectedIndex <= -1 ||
                lstServiceType.SelectedIndex <= -1 || lstServiceMode.SelectedIndex <= -1 || lstShipMode.SelectedIndex <= -1 || lstPaymentMode.SelectedIndex <= -1)
            {
                return false;
            }
            if (shipment.PackageDimensions == null)
            {
                return false;
            }
            else
            {
                if (shipment.PackageDimensions.Count <= 0)
                {
                    return false;
                }
            }
            return true;

        }
        #endregion

        #region Acceptance

        private void AcceptanceLoadInit()
        {
            shipment = new ShipmentModel();
            shipment.PackageDimensions = new List<PackageDimensionModel>();

            bsCommodityType = new BindingSource();
            bsCommodity = new BindingSource();
            bsServiceType = new BindingSource();
            bsServiceMode = new BindingSource();
            bsPaymentMode = new BindingSource();
            bsCrating = new BindingSource();
            bsPackaging = new BindingSource();
            bsGoodsDescription = new BindingSource();
            bsShipMode = new BindingSource();

            commodityTypes = new List<CommodityType>();
            commodities = new List<Commodity>();
            serviceTypes = new List<ServiceType>();
            serviceModes = new List<ServiceMode>();
            paymentModes = new List<PaymentMode>();
            shipmentBasicFees = new List<ShipmentBasicFee>();
            cratings = new List<Crating>();
            packagings = new List<Packaging>();
            goodsDescriptions = new List<GoodsDescription>();
            shipModes = new List<ShipMode>();
            paymentTerms = new List<PaymentTerm>();

            commodityTypeService = new CommodityTypeBL(GlobalVars.UnitOfWork);
            commodityService = new CommodityBL(GlobalVars.UnitOfWork);
            serviceTypeService = new ServiceTypeBL(GlobalVars.UnitOfWork);
            serviceModeService = new ServiceModeBL(GlobalVars.UnitOfWork);
            paymentModeService = new PaymentModeBL(GlobalVars.UnitOfWork);
            shipmentService = new ShipmentBL(GlobalVars.UnitOfWork);
            bookingService = new BookingBL(GlobalVars.UnitOfWork);
            bookingStatusService = new BookingStatusBL(GlobalVars.UnitOfWork);
            shipmentBasicFeeService = new ShipmentBasicFeeBL(GlobalVars.UnitOfWork);
            cratingService = new CratingBL(GlobalVars.UnitOfWork);
            packagingService = new PackagingBL(GlobalVars.UnitOfWork);
            goodsDescriptionService = new GoodsDescriptionBL(GlobalVars.UnitOfWork);
            shipModeService = new ShipModeBL(GlobalVars.UnitOfWork);
            rateMatrixService = new RateMatrixBL(GlobalVars.UnitOfWork);
            paymentTermService = new PaymentTermBL(GlobalVars.UnitOfWork);

            LogPath = AppDomain.CurrentDomain.BaseDirectory + "Logs\\";

        }

        private void AcceptanceLoadData()
        {

            commodityTypes = commodityTypeService.FilterActive().OrderBy(x => x.CommodityTypeName).ToList();
            commodities = commodityService.FilterActive().OrderBy(x => x.CommodityName).ToList();
            serviceTypes = serviceTypeService.FilterActive().OrderBy(x => x.ServiceTypeName).ToList();
            serviceModes = serviceModeService.FilterActive().OrderBy(x => x.ServiceModeName).ToList();
            paymentModes = paymentModeService.FilterActive().OrderBy(x => x.PaymentModeName).ToList();
            shipmentBasicFees = shipmentBasicFeeService.FilterActive();
            cratings = cratingService.FilterActive().OrderBy(x => x.CratingName).ToList();
            packagings = packagingService.FilterActive().OrderBy(x => x.PackagingName).ToList();
            goodsDescriptions = goodsDescriptionService.FilterActive().OrderBy(x => x.GoodsDescriptionName).ToList();
            shipModes = shipModeService.FilterActive().OrderBy(x => x.ShipModeName).ToList();
            paymentTerms = paymentTermService.FilterActive().OrderBy(x => x.PaymentTermName).ToList();

            bsCommodityType.DataSource = commodityTypes;
            bsCommodity.DataSource = commodities;
            bsServiceType.DataSource = serviceTypes;
            bsServiceMode.DataSource = serviceModes;
            bsPaymentMode.DataSource = paymentModes;
            bsCrating.DataSource = cratings;
            bsPackaging.DataSource = packagings;
            bsGoodsDescription.DataSource = goodsDescriptions;
            bsShipMode.DataSource = shipModes;

            dateAcceptedDate.Value = DateTime.Now;

            btnSave.Enabled = false;
            btnPrint.Enabled = false;
            btnPayment.Enabled = false;

            lstCommodityType.DataSource = bsCommodityType;
            lstCommodityType.DisplayMember = "CommodityTypeName";
            lstCommodityType.ValueMember = "CommodityTypeId";

            lstCommodity.DataSource = bsCommodity;
            lstCommodity.DisplayMember = "CommodityName";
            lstCommodity.ValueMember = "CommodityId";

            lstServiceType.DataSource = bsServiceType;
            lstServiceType.DisplayMember = "ServiceTypeName";
            lstServiceType.ValueMember = "ServiceTypeId";

            lstServiceMode.DataSource = bsServiceMode;
            lstServiceMode.DisplayMember = "ServiceModeName";
            lstServiceMode.ValueMember = "ServiceModeId";

            lstPaymentMode.DataSource = bsPaymentMode;
            lstPaymentMode.DisplayMember = "PaymentModeName";
            lstPaymentMode.ValueMember = "PaymentModeId";

            lstCrating.DataSource = bsCrating;
            lstCrating.DisplayMember = "CratingName";
            lstCrating.ValueMember = "CratingId";

            lstGoodsDescription.DataSource = bsGoodsDescription;
            lstGoodsDescription.DisplayMember = "GoodsDescriptionName";
            lstGoodsDescription.ValueMember = "GoodsDescriptionId";

            lstShipMode.DataSource = bsShipMode;
            lstShipMode.DisplayMember = "ShipModeName";
            lstShipMode.ValueMember = "ShipModeId";

            bsCommodityType.ResetBindings(false);
            bsCommodity.ResetBindings(false);
            bsServiceType.ResetBindings(false);
            bsServiceMode.ResetBindings(false);
            bsPaymentMode.ResetBindings(false);
            bsCrating.ResetBindings(false);
            bsPackaging.ResetBindings(false);
            bsGoodsDescription.ResetBindings(false);
            bsShipMode.ResetBindings(false);

            lstCommodityType.SelectedIndex = -1;
            lstCommodity.SelectedIndex = -1;
            lstCrating.SelectedIndex = -1;
            lstShipMode.SelectedIndex = -1;
            lstGoodsDescription.SelectedIndex = -1;

            DisableForm();
            ShowNewShipment();
        }

        public void ShowNewShipment()
        {
            if (shipmentModel != null)
            {
                shipment = new ShipmentModel();
                shipment.ShipmentId = Guid.NewGuid();
                shipment.OriginCityId = shipmentModel.OriginCityId;
                shipment.OriginCity = shipmentModel.OriginCity;
                shipment.DestinationCityId = shipmentModel.DestinationCityId;
                shipment.DestinationCity = shipmentModel.DestinationCity;
                shipment.ShipperId = shipmentModel.ShipperId;
                shipment.Shipper = shipmentModel.Shipper;
                shipment.OriginAddress = shipmentModel.OriginAddress;
                shipment.ConsigneeId = shipmentModel.ConsigneeId;
                shipment.Consignee = shipmentModel.Consignee;
                shipment.DestinationAddress = shipmentModel.DestinationAddress;
                shipment.BookingId = shipmentModel.BookingId;

                AcceptancePopulateForm();
                DisableForm();

                AcceptancetxtAirwayBill.Focus();
                btnSearchShipment.Enabled = false;
                btnAcceptanceEdit.Enabled = false;
            }
            else
            {
                AcceptancetxtAirwayBill.Focus();
                btnSearchShipment.Enabled = true;
            }
        }

        private void SaveShipment(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _worker = sender as BackgroundWorker;
            int percent = 1;
            int index = 1;
            int max = 2; // # of processes

            #region SaveShipment

            shipmentService.AddEdit(shipment);
            percent = index * 100 / max;
            _worker.ReportProgress(percent);
            index++;

            #endregion

            #region UpdateBookingStatus

            var booking = bookingService.GetById(shipment.BookingId);
            if (booking != null)
            {
                var bookingStatus = bookingStatusService.FilterActiveBy(x => x.BookingStatusName.Equals("Picked-up"));
                if (bookingStatus != null)
                {
                    booking.BookingStatusId = bookingStatus.FirstOrDefault().BookingStatusId;
                    booking.ModifiedBy = AppUser.User.UserId;
                    booking.ModifiedDate = DateTime.Now;
                    bookingService.Edit(booking);
                }
            }
            percent = index * 100 / max;
            _worker.ReportProgress(percent);
            index++;

            #endregion
        }

        private void AcceptanceResetAll()
        {
            dateAcceptedDate.Value = DateTime.Now;
            AcceptancetxtShipperAccountNo.Text = "";
            AcceptancetxtShipperFullName.Text = "";
            AcceptancetxtShipperCompany.Text = "";
            AcceptancetxtShipperAddress.Text = "";
            AcceptancetxtShipperBarangay.Text = "";
            AcceptancetxtShipperCity.Text = "";
            AcceptancetxtShipperContactNo.Value = AcceptancetxtShipperContactNo.Mask;
            AcceptancetxtShipperMobile.Value = AcceptancetxtShipperMobile.Mask;
            AcceptancetxtShipperEmail.Text = "";
            AcceptancetxtConsigneeAccountNo.Text = "";
            AcceptancetxtConsigneeFullName.Text = "";
            AcceptancetxtConsigneeCompany.Text = "";
            AcceptancetxtConsigneeAddress.Text = "";
            AcceptancetxtConsigneeBarangay.Text = "";
            AcceptancetxtConsigneeCity.Text = "";
            AcceptancetxtConsigneeContactNo.Value = AcceptancetxtConsigneeContactNo.Mask;
            AcceptancetxtConsingneeMobile.Value = AcceptancetxtConsingneeMobile.Mask;
            AcceptancetxtConsigneeEmail.Text = "";
            AcceptancetxtAirwayBill.Text = "";
            txtQuantity.Text = "1";
            txtWeight.Text = "0";
            txtWidth.Text = "0";
            txtLength.Text = "0";
            txtHeight.Text = "0";
            txtTotalEvm.Text = "";
            txtTotalWeightCharge.Text = "";
            gridPackage.DataSource = null;
            txtDeclaredValue.Text = "";
            txtTotalEvm.Text = "";
            txtTotalWeightCharge.Text = "";
            txtRfa.Text = "";
            txtHandlingFee.Text = "";
            txtQuarantineFee.Text = "";


            shipment.PackageDimensions = new List<PackageDimensionModel>();
            shipment = new ShipmentModel();

            if (lstCommodityType.Items.Count > 0)
                lstCommodityType.SelectedIndex = -1;
            if (lstCommodity.Items.Count > 0)
                lstCommodity.SelectedIndex = -1;
            if (lstServiceMode.Items.Count > 0)
                lstServiceMode.SelectedIndex = -1;
            if (lstPaymentMode.Items.Count > 0)
                lstPaymentMode.SelectedIndex = -1;
            if (lstServiceType.Items.Count > 0)
                lstServiceType.SelectedIndex = -1;
            if (lstShipMode.Items.Count > 0)
                lstShipMode.SelectedIndex = -1;
            if (lstGoodsDescription.Items.Count > 0)
                lstGoodsDescription.SelectedIndex = -1;

            txtNotes.Text = "";
            btnCompute.Enabled = false;
            btnAcceptanceSave.Enabled = false;
            btnAcceptanceEdit.Enabled = false;
            btnPrint.Enabled = false;
            btnAcceptanceEdit.Enabled = false;
            btnPayment.Enabled = false;
            btnSearchShipment.Enabled = true;

            chkNonVatable.Checked = false;

        }

        private void ComputeCharges()
        {
            shipment.DateAccepted = dateAcceptedDate.Value;
            shipment.CommodityTypeId = Guid.Parse(lstCommodityType.SelectedValue.ToString());
            shipment.CommodityType = commodityTypes.Find(x => x.CommodityTypeId == shipment.CommodityTypeId);
            shipment.ServiceTypeId = Guid.Parse(lstServiceType.SelectedValue.ToString());
            shipment.ServiceType = serviceTypes.Find(x => x.ServiceTypeId == shipment.ServiceTypeId);
            shipment.ServiceModeId = Guid.Parse(lstServiceMode.SelectedValue.ToString());
            shipment.ServiceMode = serviceModes.Find(x => x.ServiceModeId == shipment.ServiceModeId);
            shipment.ShipModeId = Guid.Parse(lstShipMode.SelectedValue.ToString());
            shipment.ShipMode = shipModes.Find(x => x.ShipModeId == shipment.ShipModeId);
            if (shipment.GoodsDescriptionId == null || shipment.GoodsDescription == null)
            {
                if (lstGoodsDescription.SelectedValue == null)
                {
                    lstGoodsDescription.SelectedIndex = 0;
                }
                shipment.GoodsDescriptionId = Guid.Parse(lstGoodsDescription.SelectedValue.ToString());
                shipment.GoodsDescription =
                    goodsDescriptions.Find(x => x.GoodsDescriptionId == shipment.GoodsDescriptionId);
            }

            if (shipment.PaymentModeId == null || shipment.PaymentMode == null)
            {
                if (lstPaymentMode.SelectedValue == null)
                {
                    lstPaymentMode.SelectedIndex = 0;
                }
                shipment.PaymentModeId = Guid.Parse(lstPaymentMode.SelectedValue.ToString());
                shipment.PaymentMode = paymentModes.Find(x => x.PaymentModeId == shipment.PaymentModeId);
            }

            try
            {
                shipment.DeclaredValue = Decimal.Parse(txtDeclaredValue.Value.ToString().Replace("₱", ""));
                shipment.HandlingFee = Decimal.Parse(txtHandlingFee.Value.ToString().Replace("₱", ""));
                shipment.QuarantineFee = Decimal.Parse(txtQuarantineFee.Value.ToString().Replace("₱", ""));
                shipment.Discount = Decimal.Parse(txtRfa.Value.ToString());
            }
            catch (Exception ex)
            {
                shipment.DeclaredValue = Decimal.Parse(txtDeclaredValue.Value.ToString().Replace("Php", ""));
                shipment.HandlingFee = Decimal.Parse(txtHandlingFee.Value.ToString().Replace("Php", ""));
                shipment.QuarantineFee = Decimal.Parse(txtQuarantineFee.Value.ToString().Replace("Php", ""));
                shipment.Discount = Decimal.Parse(txtRfa.Value.ToString());
            }

            if (shipment.Shipper != null)
            {
                if (shipment.Shipper.Company != null)
                {
                    shipment.Discount = shipment.Shipper.Company.Discount;
                }
            }

            if (chkNonVatable.Checked)
            {
                shipment.IsVatable = false;
            }
            else
            {
                shipment.IsVatable = true;
            }

            shipment = shipmentService.ComputeCharges(shipment);
            PopulateSummary();
        }

        private void AcceptancePopulateForm()
        {
            if (shipment != null)
            {
                if (shipment.Shipper != null)
                {
                    AcceptancetxtShipperAccountNo.Text = shipment.Shipper.AccountNo;
                    AcceptancetxtShipperFullName.Text = shipment.Shipper.LastName + ", " + shipment.Shipper.FirstName;
                    if (shipment.Shipper.CompanyId != null)
                    {
                        AcceptancetxtShipperCompany.Text = shipment.Shipper.Company.CompanyName;
                    }
                    else
                    {
                        AcceptancetxtConsigneeCompany.Text = "N/A";
                    }
                    AcceptancetxtShipperAddress.Text = shipment.Shipper.Address1 + ", " + shipment.Shipper.Address2;
                    AcceptancetxtShipperBarangay.Text = shipment.Shipper.Barangay;
                    AcceptancetxtShipperContactNo.Text = shipment.Shipper.ContactNo;
                    AcceptancetxtShipperMobile.Text = shipment.Shipper.Mobile;
                    AcceptancetxtShipperEmail.Text = shipment.Shipper.Email;
                }
                AcceptancetxtShipperCity.Text = shipment.OriginCity.CityName;

                if (shipment.Consignee != null)
                {
                    AcceptancetxtConsigneeAccountNo.Text = shipment.Consignee.AccountNo;
                    AcceptancetxtConsigneeFullName.Text = shipment.Consignee.LastName + ", " + shipment.Consignee.FirstName;
                    if (shipment.Consignee.CompanyId != null)
                    {
                        AcceptancetxtConsigneeCompany.Text = shipment.Consignee.Company.CompanyName;
                    }
                    else
                    {
                        AcceptancetxtConsigneeCompany.Text = "N/A";
                    }
                    AcceptancetxtConsigneeAddress.Text = shipment.Consignee.Address1 + ", " + shipment.Consignee.Address2;
                    AcceptancetxtConsigneeBarangay.Text = shipment.Consignee.Barangay;
                    AcceptancetxtConsigneeContactNo.Text = shipment.Consignee.ContactNo;
                    AcceptancetxtConsingneeMobile.Text = shipment.Consignee.Mobile;
                    AcceptancetxtConsigneeEmail.Text = shipment.Consignee.Email;
                }
                AcceptancetxtConsigneeCity.Text = shipment.DestinationCity.CityName;

                lstServiceType.SelectedIndex = -1;
                lstServiceMode.SelectedIndex = -1;
                lstPaymentMode.SelectedIndex = -1;
                if (shipment.CommodityTypeId != null && shipment.CommodityTypeId != Guid.Empty)
                    lstCommodityType.SelectedValue = shipment.CommodityTypeId;
                if (shipment.CommodityId != null && shipment.CommodityId != Guid.Empty)
                    lstCommodity.SelectedValue = shipment.CommodityId;
                if (shipment.ServiceModeId != null && shipment.ServiceModeId != Guid.Empty)
                    lstServiceMode.SelectedValue = shipment.ServiceModeId;
                if (shipment.PaymentModeId != null && shipment.ServiceModeId != Guid.Empty)
                    lstPaymentMode.SelectedValue = shipment.PaymentModeId;
                if (shipment.ServiceTypeId != null && shipment.ServiceTypeId != Guid.Empty)
                    lstServiceType.SelectedValue = shipment.ServiceTypeId;
                if (shipment.ShipModeId != null && shipment.ShipModeId != Guid.Empty)
                    lstShipMode.SelectedValue = shipment.ShipModeId;
                if (shipment.GoodsDescriptionId != null && shipment.GoodsDescriptionId != Guid.Empty)
                    lstGoodsDescription.SelectedValue = shipment.GoodsDescriptionId;
                txtQuantity.Text = shipment.Quantity.ToString();
                txtWeight.Text = shipment.Weight.ToString("N");
                txtDeclaredValue.Text = shipment.DeclaredValueString;
                txtHandlingFee.Text = shipment.HandlingFeeString;
                txtQuarantineFee.Text = shipment.QuanrantineFeeString;
                txtRfa.Text = (shipment.Discount * 100).ToString();
                txtNotes.Text = shipment.Notes;
                chkNonVatable.Checked = false;
                if (!shipment.IsVatable)
                {
                    chkNonVatable.Checked = true;
                }
                shipment.AcceptedAreaId = AppUser.User.Employee.AssignedToAreaId;
                shipment.AcceptedArea = AppUser.Employee.AssignedToArea;

            }

        }

        private void PopulateSummary()
        {
            txtSumChargeableWeight.Text = shipment.ChargeableWeightString;
            txtSumWeightCharge.Text = shipment.WeightChargeString;
            if (shipment.AwbFee != null)
            {
                txtSumAwbFee.Text = shipment.AwbFee.Amount.ToString("N");
            }
            else
            {
                txtSumAwbFee.Text = "0.00";
            }
            txtSumValuation.Text = "0.00";
            txtSumValuation.Text = shipment.ValuationAmountString;
            if (shipment.DeliveryFee != null)
                txtSumDeliveryFee.Text = shipment.DeliveryFee.AmountString;
            else
            {
                txtSumDeliveryFee.Text = "0.00";
            }
            if (shipment.FreightCollectCharge != null)
            {
                txtSumFreightCollect.Text = shipment.FreightCollectCharge.Amount.ToString("N");
            }
            else
            {
                txtSumFreightCollect.Text = "0.00";
            }
            if (shipment.PeracFee != null)
            {
                txtSumPeracFee.Text = shipment.PeracFee.Amount.ToString("N");
            }
            else
            {
                txtSumPeracFee.Text = "0.00";
            }
            if (shipment.DangerousFee != null)
            {
                txtSumDangerousFee.Text = shipment.DangerousFee.AmountString;
            }
            else
            {
                txtSumDangerousFee.Text = "0.00";
            }
            txtSumFuelSurcharge.Text = shipment.FuelSurchargeAmountstring;
            txtSumCratingFee.Text = shipment.CratingFeeString;
            txtSumDrainingFee.Text = shipment.DrainingFeeString;
            txtSumPackagingFee.Text = shipment.PackagingFeeString;
            txtSumHandlingFee.Text = shipment.HandlingFeeString;
            txtSumQuarantineFee.Text = shipment.QuanrantineFeeString;
            txtSumDiscount.Text = shipment.DiscountAmountString;
            if (shipment.Insurance != null)
            {
                txtSumInsurance.Text = shipment.InsuranceAmountString;
            }
            else
            {
                txtSumInsurance.Text = "0.00";
            }
            if (chkNonVatable.Checked)
            {
                txtSumVatAmount.Text = "0.00";
            }
            else
            {
                txtSumVatAmount.Text = shipment.ShipmentVatAmountString;
            }
            txtSumSubTotal.Text = shipment.ShipmentSubTotalString;
            txtSumTotal.Text = shipment.ShipmentTotalString;
        }
        private void AddPackage()
        {
            if (shipment.PackageDimensions == null)
                shipment.PackageDimensions = new List<PackageDimensionModel>();

            if (shipment.CommodityTypeId == null || shipment.CommodityType == null)
            {
                lstCommodityType.Focus();
                return;
            }

            if (shipment.ServiceTypeId == null || shipment.ServiceType == null)
            {
                if (lstServiceType.SelectedValue == null)
                {
                    lstServiceType.SelectedIndex = 0;
                }
                shipment.ServiceTypeId = Guid.Parse(lstServiceType.SelectedValue.ToString());
                shipment.ServiceType = serviceTypes.Find(x => x.ServiceTypeId == shipment.ServiceTypeId);
            }

            if (shipment.ServiceModeId == null || shipment.ServiceMode == null)
            {
                if (lstServiceMode.SelectedValue == null)
                {
                    lstServiceMode.SelectedIndex = 0;
                }
                shipment.ServiceModeId = Guid.Parse(lstServiceMode.SelectedValue.ToString());
                shipment.ServiceMode = serviceModes.Find(x => x.ServiceModeId == shipment.ServiceModeId);
            }

            if (shipment.ShipModeId == null || shipment.ShipMode == null)
            {
                if (lstShipMode.SelectedValue == null)
                {
                    lstShipMode.SelectedIndex = 0;
                }
                shipment.ShipModeId = Guid.Parse(lstShipMode.SelectedValue.ToString());
                shipment.ShipMode = shipModes.Find(x => x.ShipModeId == shipment.ShipModeId);
            }

            if (shipment.PaymentModeId == null || shipment.PaymentMode == null)
            {
                if (lstPaymentMode.SelectedValue == null)
                {
                    lstPaymentMode.SelectedIndex = 0;
                }
                shipment.PaymentModeId = Guid.Parse(lstPaymentMode.SelectedValue.ToString());
                shipment.PaymentMode = paymentModes.Find(x => x.PaymentModeId == shipment.PaymentModeId);
            }

            try
            {
                shipment.Quantity = Int32.Parse(txtQuantity.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Quantity.", "Data Error", MessageBoxButtons.OK);
                txtQuantity.Text = "1";
                txtQuantity.Focus();
                return;
            }
            if (shipment.Quantity <= 0)
            {
                MessageBox.Show("Invalid Quantity.", "Data Error", MessageBoxButtons.OK);
                txtQuantity.Text = "1";
                txtQuantity.Focus();
                return;
            }
            try
            {
                shipment.Weight = Decimal.Parse(txtWeight.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Weight.", "Data Error", MessageBoxButtons.OK);
                txtWeight.Text = "1";
                txtWeight.Focus();
                return;
            }
            if (shipment.Weight <= 0)
            {
                MessageBox.Show("Invalid Weight.", "Data Error", MessageBoxButtons.OK);
                txtWeight.Text = "1";
                txtWeight.Focus();
                return;
            }
            decimal length = 0;
            decimal width = 0;
            decimal height = 0;
            try
            {
                length = Decimal.Parse(txtLength.Text);
                width = Decimal.Parse(txtWidth.Text);
                height = Decimal.Parse(txtHeight.Text);
                if (!(length > 0 && width > 0 && height > 0))
                {
                    txtLength.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Dimension.", "Data Error", MessageBoxButtons.OK);
                return;
            }

            int index = 0;
            for (index = 0; index < shipment.PackageDimensions.Count; index++)
            {
                var temp = shipment.PackageDimensions.Find(x => x.Index == index);
                if (temp == null)
                {
                    break;
                }
            }
            packageDimensionModel = new PackageDimensionModel();
            packageDimensionModel.Index = index;
            packageDimensionModel.Length = length;
            packageDimensionModel.Width = width;
            packageDimensionModel.Height = height;
            packageDimensionModel.CommodityTypeId = shipment.CommodityTypeId;
            packageDimensionModel.CratingId = null;
            if (lstCrating.SelectedValue != null)
            {
                packageDimensionModel.CratingId = Guid.Parse(lstCrating.SelectedValue.ToString());
                packageDimensionModel.CratingName = lstCrating.SelectedText;
            }
            packageDimensionModel.ForPackaging = chkPackaging.Checked;
            packageDimensionModel.ForDraining = chkDraining.Checked;
            shipment.PackageDimensions.Add(packageDimensionModel);
            gridPackage.Enabled = true;
            RefreshGridPackages();
            btnCompute.Enabled = true;

            txtLength.Text = "0";
            txtWidth.Text = "0";
            txtHeight.Text = "0";
            lstCrating.SelectedIndex = -1;
            chkPackaging.Checked = false;
            chkDraining.Checked = false;
        }
        private void PrintAwb()
        {
            btnReset.Enabled = false;
            btnCompute.Enabled = false;
            btnSave.Enabled = false;
            btnPrint.Enabled = false;



            #region SetDataSet

            ShipmentPrint shipmentPrint = new ShipmentPrint();
            DataRow dr = shipmentPrint.Tables["Shipment"].NewRow();
            dr["DateAccepted"] = shipmentModel.DateAccepted.ToString("MMM dd, yyyy h:mmtt");
            dr["BranchAccepted"] = shipmentModel.AcceptedArea.RevenueUnitName;
            dr["Origin"] = shipmentModel.OriginCity.CityName;
            dr["Destination"] = shipmentModel.DestinationCity.CityName;
            dr["ShipperName"] = shipmentModel.Shipper.FullName;
            dr["ShipperAddress"] = shipmentModel.Shipper.Address1;
            dr["ConsigneeName"] = shipmentModel.Consignee.FullName;
            dr["ConsigneeAddress"] = shipmentModel.Consignee.Address1;
            dr["ServiceMode"] = shipmentModel.ServiceMode.ServiceModeName;
            dr["PaymentMode"] = shipmentModel.PaymentMode.PaymentModeCode;
            dr["DeclaredValue"] = shipmentModel.DeclaredValueString;
            dr["WeightCharge"] = shipmentModel.WeightChargeString;
            if (shipmentModel.AwbFee != null)
            {
                dr["AirwayBillFee"] = shipmentModel.AwbFee.AmountString;
            }
            else
            {
                dr["AirwayBillFee"] = "0.00";
            }
            dr["Valuation"] = shipmentModel.ValuationAmountString;
            if (shipmentModel.FreightCollectCharge != null)
            {
                dr["FreightCollect"] = shipmentModel.FreightCollectCharge.AmountString;
            }
            else
            {
                dr["FreightCollect"] = "0.00";
            }
            dr["Insurance"] = shipmentModel.InsuranceAmountString;
            dr["FuelSurcharge"] = shipmentModel.FuelSurchargeAmountstring;
            if (shipmentModel.DeliveryFee != null)
            {
                dr["DeliveryFee"] = shipmentModel.DeliveryFee.AmountString;
            }
            else
            {
                dr["DeliveryFee"] = "0.00";
            }
            if (shipmentModel.PeracFee != null)
            {
                dr["PeracFee"] = shipmentModel.PeracFee.AmountString;
            }
            else
            {
                dr["PeracFee"] = "0.00";
            }
            dr["CratingFee"] = "0.00";
            dr["SubTotal"] = shipmentModel.ShipmentSubTotalString;
            dr["VatAmount"] = shipmentModel.ShipmentVatAmountString;
            dr["Total"] = shipmentModel.ShipmentTotalString;
            dr["PreparedBy"] = shipmentModel.AcceptedBy.FullName;
            dr["AirwayBill"] = shipmentModel.AirwayBillNo;
            dr["Commodity"] = shipmentModel.CommodityType.CommodityTypeName;
            shipmentPrint.Tables["Shipment"].Rows.Add(dr);

            string dimension = "";
            foreach (var item in shipmentModel.PackageDimensions)
            {
                dimension = item.LengthString + " x " + item.WidthString + " x " + item.HeightString;
                dr = shipmentPrint.Tables["Packages"].NewRow();
                if (shipmentPrint.Tables["Packages"].Rows.Count == 0)
                {
                    dr["Quantity"] = shipmentModel.Quantity;
                    dr["ActualWeight"] = shipmentModel.Weight;
                }
                else
                {
                    dr["Quantity"] = "";
                    dr["ActualWeight"] = "";
                }
                dr["Dimension"] = dimension;
                dr["EVM"] = item.EvmString;
                dr["ChargeableWeight"] = item.WeightChargeString;
                shipmentPrint.Tables["Packages"].Rows.Add(dr);
            }

            #endregion

            try
            {
                PrinterSettings printer = new PrinterSettings();
                ReportDocument report = new ReportDocument();
                report.Load(AppDomain.CurrentDomain.BaseDirectory + "Reports\\ShipmentPrintForm.rpt");
                report.SetDataSource(shipmentPrint);
                report.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                report.PrintOptions.PrinterName = printer.PrinterName;
                report.PrintToPrinter(1, false, 0, 0);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    Logs.ErrorLogs(LogPath, "Acceptance-PrintAwb", ex.Message);
                else
                    Logs.ErrorLogs(LogPath, "Acceptance-PrintAwb", ex.InnerException.Message);
            }

            btnReset.Enabled = true;
            btnPayment.Enabled = true;
        }
        private void ProceedToPayment()
        {
            PaymentDetailsViewModel newPayment = new PaymentDetailsViewModel();
            newPayment.AwbSoa = AcceptancetxtAirwayBill.Text;
            try
            {
                newPayment.AmountPaid = decimal.Parse(txtSumTotal.Value.ToString().Replace("₱", ""));
            }
            catch (Exception ex)
            {
                newPayment.AmountPaid = decimal.Parse(txtSumTotal.Value.ToString().Replace("Php", ""));
            }


            NewPayment = newPayment;
            ((RadPageView)BookingPage.Parent).SelectedPage = this.PaymentPage;

        }
        private void RefreshOptions()
        {
            Guid commodityTypeId = new Guid();
            Guid commodityId = new Guid();
            Guid serviceTypeId = new Guid();
            Guid serviceModeId = new Guid();

            if (lstCommodityType.SelectedValue != null)
                commodityTypeId = Guid.Parse(lstCommodityType.SelectedValue.ToString());
            //if (lstCommodity.SelectedValue != null)
            //    commodityId = Guid.Parse(lstCommoditySelectedValue.ToString());
            if (lstServiceType.SelectedValue != null)
                serviceTypeId = Guid.Parse(lstServiceType.SelectedValue.ToString());
            if (lstServiceMode.SelectedValue != null)
                serviceModeId = Guid.Parse(lstServiceMode.SelectedValue.ToString());

            var matrix =
                rateMatrixService.FilterActiveBy(
                    x =>
                        x.CommodityTypeId == commodityTypeId && x.ServiceTypeId == serviceTypeId &&
                        x.ServiceModeId == serviceModeId).FirstOrDefault();

            if (matrix != null)
            {
                //shipment. = matrix.DeliveryFee;
                //shipment.DangerousFee = matrix.DangerousFee;
            }
        }
        private void RefreshGridPackages()
        {
            if (shipment.PackageDimensions != null)
            {
                //decimal totalWeightCharge = 0;
                decimal totalEvm = 0;
                if (shipment.PackageDimensions.Count > 0)
                {
                    shipment.WeightCharge = 0;
                    gridPackage.DataSource = null;
                    shipment = shipmentService.ComputePackageEvmCrating(shipment);

                    gridPackage.DataSource = ConvertToDataTable(shipment, out totalEvm);

                    gridPackage.Columns["No"].Width = 20;
                    gridPackage.Columns["Length"].Width = 70;
                    gridPackage.Columns["Width"].Width = 70;
                    gridPackage.Columns["Height"].Width = 70;
                    gridPackage.Columns["Crating"].Width = 70;
                    gridPackage.Columns["Packaging"].Width = 70;
                    gridPackage.Columns["Draining"].Width = 70;

                    gridPackage.Columns["No"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Length"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Width"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Height"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Crating"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Packaging"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                    gridPackage.Columns["Draining"].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;

                    shipment = shipmentService.ComputePackageWeightCharge(shipment);

                }
                else
                {
                    shipment.WeightCharge = 0;
                    totalEvm = 0;
                    gridPackage.DataSource = null;
                }
                txtTotalEvm.Text = totalEvm.ToString("N");
                txtTotalWeightCharge.Text = shipment.WeightChargeString;
            }
        }
        private void CommodityTypeSelected()
        {
            commodityType = new CommodityType();
            if (lstCommodityType.Items.Count > 0)
            {
                if (lstCommodityType.SelectedIndex >= 0)
                {
                    Guid commodityTypeId = Guid.Parse(lstCommodityType.SelectedValue.ToString());
                    commodityType = commodityTypes.Find(x => x.CommodityTypeId == commodityTypeId);
                    shipment.CommodityTypeId = commodityTypeId;
                    shipment.CommodityType = commodityTypes.Find(x => x.CommodityTypeId == commodityTypeId);

                    var _commodities =
                        commodities.Where(x => x.CommodityTypeId == commodityTypeId).OrderBy(x => x.CommodityName).ToList();
                    lstCommodity.DataSource = _commodities;
                    lstCommodity.DisplayMember = "CommodityName";
                    lstCommodity.ValueMember = "CommodityId";
                }
                else
                {
                    lstCommodityType.Focus();
                }

            }

            btnAddPackage.Enabled = true;

            gridPackage.ReadOnly = false;

            txtQuantity.Enabled = true;
            txtWeight.Enabled = true;
            txtLength.Enabled = true;
            txtWidth.Enabled = true;
            txtHeight.Enabled = true;
            btnAddPackage.Enabled = true;
            RefreshGridPackages();
            RefreshOptions();
            lstCommodity.Focus();
        }
        private void EnableForm()
        {
            lstCommodityType.Enabled = true;
            lstCommodity.Enabled = true;
            lstServiceType.Enabled = true;
            lstServiceMode.Enabled = true;
            lstShipMode.Enabled = true;
            txtQuantity.Enabled = true;
            txtWeight.Enabled = true;
            txtLength.Enabled = true;
            txtWidth.Enabled = true;
            txtHeight.Enabled = true;
            lstCrating.Enabled = true;
            chkPackaging.Enabled = true;
            chkDraining.Enabled = true;
            btnAddPackage.Enabled = true;
            gridPackage.Enabled = true;
            lstGoodsDescription.Enabled = true;
            lstPaymentMode.Enabled = true;
            txtTotalEvm.Enabled = true;
            txtTotalWeightCharge.Enabled = true;
            txtDeclaredValue.Enabled = true;
            txtHandlingFee.Enabled = true;
            txtQuarantineFee.Enabled = true;
            txtRfa.Enabled = true;
            chkNonVatable.Enabled = true;
            txtNotes.Enabled = true;

            btnCompute.Enabled = true;
            btnAcceptanceSave.Enabled = false;
            btnAcceptanceEdit.Enabled = false;
            btnAcceptanceReset.Enabled = true;
            btnPayment.Enabled = false;
        }
        public void DisableForm()
        {
            lstCommodityType.Enabled = false;
            lstCommodity.Enabled = false;
            lstServiceType.Enabled = false;
            lstServiceMode.Enabled = false;
            lstShipMode.Enabled = false;
            txtQuantity.Enabled = false;
            txtWeight.Enabled = false;
            txtLength.Enabled = false;
            txtWidth.Enabled = false;
            txtHeight.Enabled = false;
            lstCrating.Enabled = false;
            chkPackaging.Enabled = false;
            chkDraining.Enabled = false;
            btnAddPackage.Enabled = false;
            gridPackage.Enabled = false;
            lstGoodsDescription.Enabled = false;
            lstPaymentMode.Enabled = false;
            txtTotalEvm.Enabled = false;
            txtTotalWeightCharge.Enabled = false;
            txtDeclaredValue.Enabled = false;
            txtHandlingFee.Enabled = false;
            txtQuarantineFee.Enabled = false;
            txtRfa.Enabled = false;
            chkNonVatable.Enabled = false;
            txtNotes.Enabled = false;

            btnCompute.Enabled = false;
            btnAcceptanceSave.Enabled = false;
            btnAcceptanceEdit.Enabled = false;
            btnPayment.Enabled = false;
            btnPrint.Enabled = false;

            if (AcceptancetxtShipperAccountNo.Text != "")
            {
                btnAcceptanceEdit.Enabled = true;
            }
        }
        private void ClearSummaryData()
        {
            txtSumChargeableWeight.Text = "0.00";
            txtSumWeightCharge.Text = "0.00";
            txtSumAwbFee.Text = "0.00";
            txtSumValuation.Text = "0.00";
            txtSumDeliveryFee.Text = "0.00";
            txtSumFreightCollect.Text = "0.00";
            txtSumPeracFee.Text = "0.00";
            txtSumFuelSurcharge.Text = "0.00";
            txtSumDangerousFee.Text = "0.00";
            txtSumCratingFee.Text = "0.00";
            txtSumDrainingFee.Text = "0.00";
            txtSumPackagingFee.Text = "0.00";
            txtSumHandlingFee.Text = "0.00";
            txtSumQuarantineFee.Text = "0.00";
            txtSumDiscount.Text = "0.00";
            txtSumInsurance.Text = "0.00";
            txtSumSubTotal.Text = "0.00";
            txtSumVatAmount.Text = "0.00";
            txtSumTotal.Text = "0.00";
            txtAmountDue.Text = "0.00";
            txtAmountPaid.Text = "0.00";
        }
        #endregion

        #region Payment

        private void PaymentReset()
        {
            txtSoaNo.Text = "";
            txtSoaNo.Enabled = true;
            txtAwb.Text = "";
            txtAwb.Enabled = true;
            txtOrNo.Text = "";
            txtAmountPaid.Text = "";
            txtAmountDue.Text = "";
            txtPrNo.Text = "";
            txtAmountDue.Text = txtAmountDue.Mask;
            txtAmountPaid.Text = txtAmountPaid.Mask;
            txtNetCollection.Text = txtNetCollection.Mask;
            txtTaxWithheld.Text = txtTaxWithheld.Mask;
            datePaymentDate.Value = DateTime.Now;
            txtAmountPaid.Text = txtAmountPaid.Mask;
            txtTaxWithheld.Text = txtTaxWithheld.Mask;
            txtNetCollection.Text = txtNetCollection.Mask;
            lstPaymentType.Text = "Cash";
            txtCheckBank.Text = "";
            txtCheckBank.Enabled = false;
            txtCheckNo.Text = "";
            txtCheckNo.Enabled = false;
            dateCheckDate.Value = DateTime.Now;
            dateCheckDate.Enabled = false;
            cmb_PaymentRemarks.Text = "";
        }

        private void SavePayment(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _worker = sender as BackgroundWorker;
            int percent = 1;
            int index = 1;
            int max = 2; // # of processes

            if (soaPayment != null)
            {
                soaService.MakePayment(soaPayment, soaService.EntityToModel(soa));
                percent = index * 100 / max;
                _worker.ReportProgress(percent);
                index++;
            }
            if (payment != null)
            {
                paymentService.Add(payment);
                percent = index * 100 / max;
                _worker.ReportProgress(percent);
                index++;
            }
        }

        private void ComputeNetCollection()
        {
            try
            {
                txtNetCollection.Text = (decimal.Parse(txtAmountPaid.Value.ToString().Replace("₱", "")) - (decimal.Parse(txtTaxWithheld.Value.ToString().Replace("₱", "")))).ToString();
            }
            catch (Exception)
            {
                txtNetCollection.Text = (decimal.Parse(txtAmountPaid.Value.ToString().Replace("Php", "")) - (decimal.Parse(txtTaxWithheld.Value.ToString().Replace("Php", "")))).ToString();

            }
        }

        private void LoadPayment()
        {
            if (shipment != null)
            {
                txtAmountDue.Text = Convert.ToString(shipment.ShipmentTotal);
            }
        }


        #endregion

        #region Manifest

        #endregion

        #region PaymentSummary

        public void PaymentSummaryLoadInit()
        {
            bsRevenueUnitType = new BindingSource();

            employees = new List<Employee>();
            paymentPrepaid = new List<Payment>();
            paymentFreightCollect = new List<Payment>();
            paymentCorpAcctConsignee = new List<Payment>();
            paymentSummary_revenueUnits = new List<RevenueUnit>();
            paymentSummary_revenueUnitType = new List<RevenueUnitType>();
            paymentSummary_employee = new List<Employee>();

            employeeService = new EmployeeBL(GlobalVars.UnitOfWork);
            paymentService = new PaymentBL(GlobalVars.UnitOfWork);
            revenueUnitTypeService = new RevenueUnitTypeBL(GlobalVars.UnitOfWork);
            revenueUnitservice = new RevenueUnitBL(GlobalVars.UnitOfWork);
            paymentSummaryStatusService = new PaymentSummaryStatusBL(GlobalVars.UnitOfWork);
            paymentSummaryService = new PaymentSummaryBL(GlobalVars.UnitOfWork);

        }
        public void PaymentSummaryLoadData()
        {
            dateCollectionDate.Value = DateTime.Now;
            List<RevenueUnitType> _revenueUnitType = revenueUnitTypeService.GetAll().Where(x => x.RecordStatus == 1).ToList();
            bsRevenueUnitType.DataSource = _revenueUnitType;
            lstRevenueUnitType.DataSource = bsRevenueUnitType;
            lstRevenueUnitType.DisplayMember = "RevenueUnitTypeName";
            lstRevenueUnitType.ValueMember = "RevenueUnitTypeId";

            if (lstRevenueUnitType.SelectedIndex == 0)
            {
                Guid revenueUnitTypeId = new Guid();
                try
                {
                    revenueUnitTypeId = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedRevenueUnit(revenueUnitTypeId);
            }

            if (lstRevenueUnitName.SelectedIndex == 0)
            {
                Guid revenueUnitId = new Guid();
                try
                {
                    revenueUnitId = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());
                }
                catch (Exception)
                {
                    return;
                }

                SelectedUser(revenueUnitId);
            }
            btnPrintPaymentSummary.Enabled = false;

        }

        private void clearPaymentSummaryData()
        {
            totalCashReceived = 0;
            totalCheckReceived = 0;
            totalAmountReceived = 0;
            difference = 0;
            txtTotalCashReceived.Text = totalCashReceived.ToString();
            txtTotalCheckReceived.Text = totalCheckReceived.ToString();
            txtTotalAmntReceived.Text = totalAmountReceived.ToString();
            txtDifference.Text = difference.ToString();
        }
        private void clearSummaryData()
        {
            if (img_Signature.Image != null)
            {
                img_Signature.Image = null;
                Invalidate();
            }

            txtTotalCash.Text = "";
            txtTotalCheck.Text = "";
            txtTotalCollection.Text = "";
            txtTotalTax.Text = "";
            txtTotalPdc.Text = "";
            txtTotalCashReceived.Text = "";
            txtTotalCheckReceived.Text = "";
            txtTotalAmntReceived.Text = "";
            txtDifference.Text = "";
            txtRemarksPaymentSummary.Text = "";

        }
        private void clearListofPaymentSummary()
        {
            listPaymentSummary = new List<PaymentSummaryModel>();
            listpaymentSummaryDetails = new List<PaymentSummaryDetails>();
            listMainDetails = new List<PaymentSummary_MainDetailsModel>();
            passListofPaymentSummary = new List<PaymentSummaryModel>();
        }
        public Tuple<Guid, Guid, Guid> getData()
        {
            Guid revenueUnitTypeId = new Guid();
            Guid revenueUnitId = new Guid();
            Guid userId = new Guid();
            if (lstRevenueUnitType.SelectedIndex >= 0 && lstRevenueUnitName.SelectedIndex >= 0 && lstUser.SelectedIndex >= 0)
            {
                try
                {
                    revenueUnitTypeId = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                    revenueUnitId = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());
                    userId = Guid.Parse(lstUser.SelectedValue.ToString());
                }
                catch (Exception)
                {

                }
            }

            return new Tuple<Guid, Guid, Guid>(revenueUnitTypeId, revenueUnitId, userId);
        }
        public Tuple<Guid, Guid> getListData()
        {
            Guid revenueUnitTypeId = new Guid();
            Guid revenueUnitId = new Guid();
            if (lstRevenueUnitType.SelectedIndex >= 0 && lstRevenueUnitName.SelectedIndex >= 0 && lstUser.SelectedIndex >= 0)
            {
                try
                {
                    revenueUnitTypeId = Guid.Parse(lstRevenueUnitType.SelectedValue.ToString());
                    revenueUnitId = Guid.Parse(lstRevenueUnitName.SelectedValue.ToString());

                }
                catch (Exception)
                {

                }
            }

            return new Tuple<Guid, Guid>(revenueUnitTypeId, revenueUnitId);
        }
        public void addCheckboxPrepaid()
        {
            GridViewCheckBoxColumn checkBoxColumn = new GridViewCheckBoxColumn();
            checkBoxColumn.DataType = typeof(bool);
            checkBoxColumn.FieldName = "Validate";
            checkBoxColumn.HeaderText = "Validate";
            checkBoxColumn.ReadOnly = false;
            gridPrepaid.MasterTemplate.Columns.Add(checkBoxColumn);

            ctrPrepaid = 1;
        }
        public void addCheckboxFreight()
        {
            GridViewCheckBoxColumn checkBoxColumn = new GridViewCheckBoxColumn();
            checkBoxColumn.DataType = typeof(bool);
            checkBoxColumn.FieldName = "Validate";
            checkBoxColumn.HeaderText = "Validate";
            checkBoxColumn.ReadOnly = false;
            gridFreightCollect.MasterTemplate.Columns.Add(checkBoxColumn);

            ctrfreight = 1;
        }
        public void addCheckboxcorpAccount()
        {
            GridViewCheckBoxColumn checkBoxColumn = new GridViewCheckBoxColumn();
            checkBoxColumn.DataType = typeof(bool);
            checkBoxColumn.FieldName = "Validate";
            checkBoxColumn.HeaderText = "Validate";
            checkBoxColumn.ReadOnly = false;
            gridCorpAcctConsignee.MasterTemplate.Columns.Add(checkBoxColumn);

            ctrcorpAcct = 1;
        }
        private void PopulateGrid_Prepaid(List<Payment> pp_payment = null)
        {
            List<Payment> _getAllPaymentPrepaid = new List<Payment>();
            List<Payment> _prepaidPayment;
            List<Entities.PaymentSummary> _paymentSummaryprepaid;

            String dt = dateCollectionDate.Text.ToString();
            DateTime date = Convert.ToDateTime(dt);

            decimal _totalAmountPaid;
            decimal _totalTaxwithheld;
            Guid revenueUnitTypeId = new Guid();
            Guid revenueUnitId = new Guid();
            Guid userId = new Guid();

            string user = lstUser.Text;
            if (user.Equals("All"))
            {
                var lisData = getListData();
                revenueUnitTypeId = lisData.Item1;
                revenueUnitId = lisData.Item2;
                string code = "PP";

                if (pp_payment == null)
                {
                    _getAllPaymentPrepaid = getAllPayment(code, date, revenueUnitId, revenueUnitTypeId);
                }
                else
                {
                    _prepaidPayment = pp_payment;
                }
            }
            else
            {
                var tuple = getData();
                revenueUnitTypeId = tuple.Item1;
                revenueUnitId = tuple.Item2;
                userId = tuple.Item3;

                if (pp_payment == null)
                {
                    _getAllPaymentPrepaid = paymentService.FilterActive().Where(x => x.Shipment.PaymentMode.PaymentModeCode == "PP"
                                     && x.PaymentDate.Date == date.Date && x.ReceivedBy.AssignedToAreaId == revenueUnitId
                                     && x.ReceivedBy.AssignedToArea.RevenueUnitTypeId == revenueUnitTypeId
                                     && x.ReceivedBy.EmployeeId == userId)
                                     .OrderBy(x => x.CreatedDate).ToList();
                }
                else
                {
                    _prepaidPayment = pp_payment;
                }
            }

            _paymentSummaryprepaid = paymentSummaryService.FilterActive().OrderBy(x => x.CreatedDate).ToList();

            _prepaidPayment = _getAllPaymentPrepaid.Where(p => !_paymentSummaryprepaid.Any(p2 => p2.PaymentId == p.PaymentId)).ToList();

            _totalAmountPaid = _prepaidPayment.Select(x => x.Amount).Sum();
            _totalTaxwithheld = _prepaidPayment.Select(x => x.TaxWithheld).Sum();
            txtTotalAmntPrepaid.Text = _totalAmountPaid.ToString();
            txtTotalTaxPrepaid.Text = _totalTaxwithheld.ToString();

            paymentPrepaid = _prepaidPayment;
            gridPrepaid.DataSource = ConvertToDataTable(_prepaidPayment);

            if (ctrPrepaid != 1)
            {

                addCheckboxPrepaid();
            }


            gridPrepaid.Columns["PaymentId"].IsVisible = false;
            gridPrepaid.Columns["ClientId"].IsVisible = false;
            gridPrepaid.Columns["Client"].Width = 100;
            gridPrepaid.Columns["AWB No"].Width = 100;
            gridPrepaid.Columns["Payment Type"].Width = 150;
            gridPrepaid.Columns["Amount Due"].Width = 150;
            gridPrepaid.Columns["Amount Paid"].Width = 100;
            gridPrepaid.Columns["Tax Withheld"].Width = 150;
            gridPrepaid.Columns["OR No"].Width = 150;
            gridPrepaid.Columns["PR No"].Width = 150;
            gridPrepaid.Columns["Status"].Width = 150;
            gridPrepaid.Columns["Collected By"].Width = 150;
            gridPrepaid.Columns["ValidatedById"].IsVisible = false;
            gridPrepaid.Columns["Validated By"].Width = 150;
            gridPrepaid.Refresh();

        }
        public List<Payment> getAllPayment(string code, DateTime date, Guid revenueUnitId, Guid revenueUnitTypeId)
        {
            List<Payment> _getAllPayment;

            _getAllPayment = paymentService.FilterActive().Where(x => x.Shipment.PaymentMode.PaymentModeCode == code
                                && x.PaymentDate.Date == date.Date && x.ReceivedBy.AssignedToAreaId == revenueUnitId
                                && x.ReceivedBy.AssignedToArea.RevenueUnitTypeId == revenueUnitTypeId)
                                .OrderBy(x => x.CreatedDate).ToList();

            return _getAllPayment;
        }
        private void PopulateGrid_FreightCollect(List<Payment> fc_payment = null)
        {
            List<Payment> _getAllPaymentfreightCollect = new List<Payment>();
            List<Payment> _fcPayment;
            List<Entities.PaymentSummary> _paymentSummaryfreightCollect;

            decimal _totalAmountPaid;
            decimal _totalTaxwithheld;
            Guid revenueUnitTypeId = new Guid();
            Guid revenueUnitId = new Guid();
            Guid userId = new Guid();

            String dt = dateCollectionDate.Text.ToString();
            DateTime date = Convert.ToDateTime(dt);

            string user = lstUser.Text;
            if (user.Equals("All"))
            {
                var lisData = getListData();
                revenueUnitTypeId = lisData.Item1;
                revenueUnitId = lisData.Item2;
                string code = "FC";

                if (fc_payment == null)
                {
                    _getAllPaymentfreightCollect = getAllPayment(code, date, revenueUnitId, revenueUnitTypeId);
                }
                else
                {
                    _fcPayment = fc_payment;
                }
            }
            else
            {
                var tuple = getData();
                revenueUnitTypeId = tuple.Item1;
                revenueUnitId = tuple.Item2;
                userId = tuple.Item3;

                if (fc_payment == null)
                {
                    _getAllPaymentfreightCollect = paymentService.FilterActive().
                                    Where(x => x.Shipment.PaymentMode.PaymentModeCode == "FC"
                                    && x.PaymentDate.Date == date.Date && x.ReceivedBy.AssignedToAreaId == revenueUnitId
                                    && x.ReceivedBy.AssignedToArea.RevenueUnitTypeId == revenueUnitTypeId
                                    && x.ReceivedBy.EmployeeId == userId)
                                    .OrderBy(x => x.CreatedDate).ToList();

                }
                else
                {
                    _fcPayment = fc_payment;
                }
            }

            _paymentSummaryfreightCollect = paymentSummaryService.FilterActive().OrderBy(x => x.CreatedDate).ToList();

            _fcPayment = _getAllPaymentfreightCollect.Where(p => !_paymentSummaryfreightCollect.Any(p2 => p2.PaymentId == p.PaymentId)).ToList();


            _totalAmountPaid = _fcPayment.Select(x => x.Amount).Sum();
            _totalTaxwithheld = _fcPayment.Select(x => x.TaxWithheld).Sum();
            txtTotalAmntFreightCollect.Text = _totalAmountPaid.ToString();
            txtTotalTaxFreightCollect.Text = _totalTaxwithheld.ToString();

            paymentFreightCollect = _fcPayment;

            gridFreightCollect.DataSource = ConvertToDataTable(_fcPayment);
            if (ctrfreight != 1)
            {

                addCheckboxFreight();
            }


            gridFreightCollect.Columns["PaymentId"].IsVisible = false;
            gridFreightCollect.Columns["ClientId"].IsVisible = false;
            gridFreightCollect.Columns["Client"].Width = 100;
            gridFreightCollect.Columns["AWB No"].Width = 100;
            gridFreightCollect.Columns["Payment Type"].Width = 150;
            gridFreightCollect.Columns["Amount Due"].Width = 150;
            gridFreightCollect.Columns["Amount Paid"].Width = 100;
            gridFreightCollect.Columns["Tax Withheld"].Width = 150;
            gridFreightCollect.Columns["OR No"].Width = 150;
            gridFreightCollect.Columns["PR No"].Width = 150;
            gridFreightCollect.Columns["Status"].Width = 150;
            gridFreightCollect.Columns["Collected By"].Width = 150;
            gridFreightCollect.Columns["ValidatedById"].IsVisible = false;
            gridFreightCollect.Columns["Validated By"].Width = 150;
            gridFreightCollect.Refresh();

        }
        private void PopulateGrid_CorpAcctConsignee(List<Payment> cac_payment = null)
        {
            List<Payment> _cacPayment;
            List<Payment> _getAllPaymentcorpAcctConsignee = new List<Payment>();
            List<Entities.PaymentSummary> _paymentSummarycorpAcctConsignee;

            decimal _totalAmountPaid;
            decimal _totalTaxwithheld;

            Guid revenueUnitTypeId = new Guid();
            Guid revenueUnitId = new Guid();
            Guid userId = new Guid();

            String dt = dateCollectionDate.Text.ToString();
            DateTime date = Convert.ToDateTime(dt);

            string user = lstUser.Text;
            if (user.Equals("All"))
            {
                var lisData = getListData();
                revenueUnitTypeId = lisData.Item1;
                revenueUnitId = lisData.Item2;
                string code = "CAC";

                if (cac_payment == null)
                {
                    _getAllPaymentcorpAcctConsignee = getAllPayment(code, date, revenueUnitId, revenueUnitTypeId);
                }
                else
                {
                    _cacPayment = cac_payment;
                }
            }
            else
            {
                var tuple = getData();
                revenueUnitTypeId = tuple.Item1;
                revenueUnitId = tuple.Item2;
                userId = tuple.Item3;


                if (cac_payment == null)
                {
                    _getAllPaymentcorpAcctConsignee = paymentService.FilterActive().
                                   Where(x => x.Shipment.PaymentMode.PaymentModeCode == "CAC"
                                   && x.PaymentDate.Date == date.Date && x.ReceivedBy.AssignedToAreaId == revenueUnitId
                                   && x.ReceivedBy.AssignedToArea.RevenueUnitTypeId == revenueUnitTypeId
                                   && x.ReceivedBy.EmployeeId == userId)
                                   .OrderBy(x => x.CreatedDate).ToList();
                }
                else
                {
                    _cacPayment = cac_payment;
                }
            }


            _paymentSummarycorpAcctConsignee = paymentSummaryService.FilterActive().OrderBy(x => x.CreatedDate).ToList();

            _cacPayment = _getAllPaymentcorpAcctConsignee.Where(p => !_paymentSummarycorpAcctConsignee.Any(p2 => p2.PaymentId == p.PaymentId)).ToList();


            _totalAmountPaid = _cacPayment.Select(x => x.Amount).Sum();
            _totalTaxwithheld = _cacPayment.Select(x => x.TaxWithheld).Sum();
            txtTotalAmntCorpAcctConsignee.Text = _totalAmountPaid.ToString();
            txtTotalTaxCorpAcctConsignee.Text = _totalTaxwithheld.ToString();

            paymentCorpAcctConsignee = _cacPayment;

            gridCorpAcctConsignee.DataSource = ConvertToDataTable(_cacPayment);
            if (ctrcorpAcct != 1)
            {

                addCheckboxcorpAccount();
            }

            gridCorpAcctConsignee.Columns["PaymentId"].IsVisible = false;
            gridCorpAcctConsignee.Columns["ClientId"].IsVisible = false;
            gridCorpAcctConsignee.Columns["Client"].Width = 100;
            gridCorpAcctConsignee.Columns["AWB No"].Width = 100;
            gridCorpAcctConsignee.Columns["Payment Type"].Width = 100;
            gridCorpAcctConsignee.Columns["Amount Due"].Width = 150;
            gridCorpAcctConsignee.Columns["Amount Paid"].Width = 100;
            gridCorpAcctConsignee.Columns["Tax Withheld"].Width = 150;
            gridCorpAcctConsignee.Columns["OR No"].Width = 150;
            gridCorpAcctConsignee.Columns["PR No"].Width = 150;
            gridCorpAcctConsignee.Columns["Status"].Width = 150;
            gridCorpAcctConsignee.Columns["Collected By"].Width = 150;
            gridCorpAcctConsignee.Columns["ValidatedById"].IsVisible = false;
            gridCorpAcctConsignee.Columns["Validated By"].Width = 150;
            gridCorpAcctConsignee.Refresh();

        }
        private DataTable ConvertToDataTable(List<Payment> list)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("PaymentId", typeof(string)));
            dt.Columns.Add(new DataColumn("ClientId", typeof(string)));
            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("AWB No", typeof(string)));
            dt.Columns.Add(new DataColumn("Payment Type", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount Due", typeof(string)));
            dt.Columns.Add(new DataColumn("Amount Paid", typeof(string)));
            dt.Columns.Add(new DataColumn("Tax Withheld", typeof(string)));
            dt.Columns.Add(new DataColumn("OR No", typeof(string)));
            dt.Columns.Add(new DataColumn("PR No", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));
            dt.Columns.Add(new DataColumn("Collected By", typeof(string)));
            dt.Columns.Add(new DataColumn("ValidatedById", typeof(string)));
            dt.Columns.Add(new DataColumn("Validated By", typeof(string)));

            dt.BeginLoadData();

            foreach (Payment item in list)
            {
                DataRow row = dt.NewRow();
                row["PaymentId"] = item.PaymentId.ToString();
                row["ClientId"] = item.Shipment.Shipper.ClientId.ToString();
                row["Client"] = item.Shipment.Shipper.FullName.ToString();
                row["AWB No"] = item.Shipment.AirwayBillNo.ToString();
                row["Payment Type"] = item.PaymentType.PaymentTypeName.ToString();
                row["Amount Due"] = item.Shipment.TotalAmount.ToString();
                row["Amount Paid"] = item.Amount.ToString();
                row["Tax Withheld"] = item.TaxWithheld.ToString();
                row["OR No"] = item.OrNo.ToString();
                row["PR No"] = item.PrNo.ToString();
                row["Status"] = "Posted";
                row["Collected By"] = item.ReceivedBy.FullName.ToString();
                row["ValidatedById"] = AppUser.Employee.EmployeeId.ToString();
                row["Validated By"] = AppUser.Employee.FullName;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();

            GridViewCheckBoxColumn checkBoxColumn = new GridViewCheckBoxColumn();
            gridPrepaid.MasterTemplate.Columns.Remove(checkBoxColumn);
            checkBoxColumn = new GridViewCheckBoxColumn();
            gridFreightCollect.MasterTemplate.Columns.Remove(checkBoxColumn);
            checkBoxColumn = new GridViewCheckBoxColumn();
            gridCorpAcctConsignee.MasterTemplate.Columns.Remove(checkBoxColumn);
            gridPrepaid.Refresh();
            gridFreightCollect.Refresh();
            gridCorpAcctConsignee.Refresh();

            return dt;
        }
        private void SelectedRevenueUnit(Guid revenueUnitTypeId)
        {
            lstRevenueUnitName.DataSource = null;
            List<RevenueUnit> _revenueUnit = revenueUnitservice.GetAll().Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.RevenueUnitTypeId == revenueUnitTypeId).OrderBy(x => x.RevenueUnitName).ToList();
            lstRevenueUnitName.DataSource = _revenueUnit;
            lstRevenueUnitName.DisplayMember = "RevenueUnitName";
            lstRevenueUnitName.ValueMember = "RevenueUnitId";

        }
        private void SelectedUser(Guid revenueUnitId)
        {
            lstUser.DataSource = null;
            List<Employee> _employee = employeeService.GetAll().Where(x => x.AssignedToArea.City.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.AssignedToArea.RevenueUnitId == revenueUnitId).ToList();
            List<Employee> _remittedBy = employeeService.GetAll().Where(x => x.AssignedToArea.City.BranchCorpOfficeId == GlobalVars.DeviceBcoId && x.AssignedToArea.RevenueUnitId == revenueUnitId).ToList();

            lstUser.DataSource = _employee;
            lstUser.DisplayMember = "FullName";
            lstUser.ValueMember = "EmployeeId";

            if (_employee != null)
            {
                lstUser.Items.Add("All");
            }

            lstRemittedBy.DataSource = _remittedBy;
            lstRemittedBy.DisplayMember = "FullName";
            lstRemittedBy.ValueMember = "EmployeeId";
        }
        private Tuple<decimal, decimal, decimal> SubtotalPrepaid()
        {
            decimal _totalCash;
            decimal _totalCheck;
            decimal _totaltax;

            _totalCash = paymentPrepaid.Where(x => x.PaymentType.PaymentTypeName == "Cash").Select(x => x.Amount).Sum();
            _totalCheck = paymentPrepaid.Where(x => x.PaymentType.PaymentTypeName == "Check").Select(x => x.Amount).Sum();
            _totaltax = paymentPrepaid.Select(x => x.TaxWithheld).Sum();

            return new Tuple<decimal, decimal, decimal>(_totalCash, _totalCheck, _totaltax);
        }
        private Tuple<decimal, decimal, decimal> SubtotalFreightCollect()
        {
            decimal _totalCash;
            decimal _totalCheck;
            decimal _totaltax;

            _totalCash = paymentFreightCollect.Where(x => x.PaymentType.PaymentTypeName == "Cash").Select(x => x.Amount).Sum();
            _totalCheck = paymentFreightCollect.Where(x => x.PaymentType.PaymentTypeName == "Check").Select(x => x.Amount).Sum();
            _totaltax = paymentFreightCollect.Select(x => x.TaxWithheld).Sum();

            return new Tuple<decimal, decimal, decimal>(_totalCash, _totalCheck, _totaltax);
        }
        private Tuple<decimal, decimal, decimal> SubtotalCAC()
        {
            decimal _totalCash;
            decimal _totalCheck;
            decimal _totaltax;

            _totalCash = paymentCorpAcctConsignee.Where(x => x.PaymentType.PaymentTypeName == "Cash").Select(x => x.Amount).Sum();
            _totalCheck = paymentCorpAcctConsignee.Where(x => x.PaymentType.PaymentTypeName == "Check").Select(x => x.Amount).Sum();
            _totaltax = paymentCorpAcctConsignee.Select(x => x.TaxWithheld).Sum();

            return new Tuple<decimal, decimal, decimal>(_totalCash, _totalCheck, _totaltax);
        }
        private void TotalPaymentSummary()
        {
            decimal totalCash;
            decimal totalCheck;
            // decimal totalCollection;
            decimal totalTaxWithheld;


            //Prepaid
            var prepaid = SubtotalPrepaid();
            decimal cashPrepaid = prepaid.Item1;
            decimal checkPrepaid = prepaid.Item2;
            decimal taxPrepaid = prepaid.Item3;

            //Freight Collect
            var freightCollect = SubtotalFreightCollect();
            decimal cashFreightCollect = freightCollect.Item1;
            decimal checkFreightCollect = freightCollect.Item2;
            decimal taxFreightCollec = freightCollect.Item3;

            //CAC
            var corpAcctConsignee = SubtotalCAC();
            decimal cashCac = corpAcctConsignee.Item1;
            decimal checkCac = corpAcctConsignee.Item2;
            decimal taxCac = corpAcctConsignee.Item3;

            totalCash = cashPrepaid + cashCac + cashFreightCollect;
            totalCheck = checkPrepaid + checkCac + checkFreightCollect;
            totalCollection = totalCash + totalCheck;
            totalTaxWithheld = taxPrepaid + taxCac + taxFreightCollec;

            txtTotalCash.Text = totalCash.ToString();
            txtTotalCheck.Text = totalCheck.ToString();
            txtTotalCollection.Text = totalCollection.ToString();
            txtTotalTax.Text = totalTaxWithheld.ToString();



        }
        public List<PaymentSummary_MainDetailsModel> amountPaymentSummary()
        {
            String dt = dateCollectionDate.Text.ToString();
            string collectedBy = lstUser.Text;
            string Area = lstRevenueUnitName.Text;
            decimal TotalCash = Convert.ToDecimal(txtTotalCash.Text);
            decimal TotalCheck = Convert.ToDecimal(txtTotalCheck.Text);
            decimal TotalCollection = Convert.ToDecimal(txtTotalCollection.Text);
            decimal TotalTaxWithheld = Convert.ToDecimal(txtTotalTax.Text);
            decimal TotalPDC;
            decimal TotalCashReceived = Convert.ToDecimal(txtTotalCashReceived.Text);
            decimal TotalCheckReceived = Convert.ToDecimal(txtTotalCheckReceived.Text);
            decimal TotalAmountReceived = Convert.ToDecimal(txtTotalAmntReceived.Text);
            decimal Difference = Convert.ToDecimal(txtDifference.Text);
            string remittedBy = lstRemittedBy.Text;

            if (!string.IsNullOrEmpty(txtTotalPdc.Text))
            {
                TotalPDC = Convert.ToDecimal(txtTotalPdc.Text);
            }
            else
            {
                TotalPDC = 0;
            }

            byte[] byteArray = imgToByteArray(signatureImage);


            PaymentSummary_MainDetailsModel mainDetails = new PaymentSummary_MainDetailsModel();
            mainDetails.CollectionDate = dt;
            mainDetails.CollectedBy = collectedBy;
            mainDetails.Area = Area;
            mainDetails.TotalCash = TotalCash;
            mainDetails.TotalCheck = TotalCheck;
            mainDetails.TotalCollection = TotalCollection;
            mainDetails.TotalTaxWithheld = TotalTaxWithheld;
            mainDetails.TotalPDC = TotalPDC;
            mainDetails.TotalCashReceived = TotalCashReceived;
            mainDetails.TotalCheckReceived = TotalCheckReceived;
            mainDetails.TotalAmountReceived = TotalAmountReceived;
            mainDetails.Difference = Difference;
            mainDetails.RemittedBy = remittedBy;
            mainDetails.Signature = byteArray;
            mainDetails.ValidatedBy = AppUser.Employee.FullName;
            listMainDetails.Add(mainDetails);

            return listMainDetails;

        }
        public List<PaymentSummaryModel> listofPaymentSummary(Guid clientId, Guid paymentId, Guid validatedById, string paymentModeCode)
        {
            paymentSummary = new Entities.PaymentSummary();
            Guid paymentStatusId = new Guid();
            Guid checkById = new Guid();
            paymentStatusId = paymentSummaryStatusService.GetAll().Where(x => x.PaymentSummaryStatusName == "Validated").Select(x => x.PaymentSummaryStatusId).First();
            checkById = userService.GetAllActiveUsers().Where(x => x.UserId == AppUser.User.UserId).Select(x => x.EmployeeId).First();

            Guid remittedById = Guid.Parse(lstRemittedBy.SelectedValue.ToString());

            PaymentSummaryModel paymentSummarymodel = new PaymentSummaryModel();
            paymentSummarymodel.PaymentSummaryId = paymentSummary.PaymentSummaryId;
            paymentSummarymodel.ClientId = clientId;
            paymentSummarymodel.PaymentId = paymentId;
            paymentSummarymodel.CheckedBy = checkById;
            paymentSummarymodel.ValidatedBy = validatedById;
            paymentSummarymodel.RemittedBy = remittedById;
            paymentSummarymodel.PaymentSummaryStatusId = paymentStatusId;
            paymentSummarymodel.DateAccepted = DateTime.Now;
            paymentSummarymodel.Remarks = txtRemarksPaymentSummary.Text.Trim();
            paymentSummarymodel.Signature = null;
            paymentSummarymodel.CreatedDate = DateTime.Now;
            paymentSummarymodel.CreatedBy = AppUser.User.UserId;
            paymentSummarymodel.ModifiedBy = AppUser.User.UserId;
            paymentSummarymodel.ModifiedDate = DateTime.Now;
            paymentSummarymodel.RecordStatus = (int)RecordStatus.Active;
            paymentSummarymodel.PaymentModeCode = paymentModeCode;
            listPaymentSummary.Add(paymentSummarymodel);

            return listPaymentSummary;
        }
        public List<PaymentSummaryDetails> summaryDetails(string AwbNo, string ClientName, string PaymentTypeName,
          decimal AmountDue, decimal AmountPaid, decimal taxWithheld, string OrNo, string PrNo,
          string ValidatedBy, string PaymentCode)
        {
            PaymentSummaryDetails details = new PaymentSummaryDetails();
            details.AwbNo = AwbNo;
            details.ClientName = ClientName;
            details.PaymentTypeName = PaymentTypeName;
            details.AmountDue = AmountDue;
            details.AmountPaid = AmountPaid;
            details.taxWithheld = taxWithheld;
            details.OrNo = OrNo;
            details.PrNo = PrNo;
            details.ValidatedBy = ValidatedBy;
            details.PaymentCode = PaymentCode;
            details.Status = "Validated";
            listpaymentSummaryDetails.Add(details);

            return listpaymentSummaryDetails;

        }
        //convert image to bytearray
        public byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                return mStream.ToArray();
            }
        }
        private void updateListofPaymentSummary(PaymentSummaryModel pSummary)
        {
            byte[] byteArray = imgToByteArray(signatureImage);

            pSummary.Signature = byteArray;
            pSummary.Remarks = txtRemarksPaymentSummary.Text.Trim();
        }
        private void SavepaymentSummary(List<PaymentSummaryModel> listofPaymentSummary)
        {
            paymentSummary = new Entities.PaymentSummary();
            foreach (PaymentSummaryModel paySummarymodel in listofPaymentSummary)
            {
                updateListofPaymentSummary(paySummarymodel);
            }

            passListofPaymentSummary = listofPaymentSummary;
        }
        private void SavingofPaymentSummary(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker _worker = sender as BackgroundWorker;
            int percent = 1;
            int index = 1;
            int max = 2; // # of processes

            if (passListofPaymentSummary != null)
            {
                List<Entities.PaymentSummary> results = paymentSummary.modelToEntity(passListofPaymentSummary);
                paymentSummaryService.AddMultiple(results);
                percent = index * 100 / max;
                _worker.ReportProgress(percent);
                index++;
            }
        }

        #endregion

        #endregion

        #region MARK SANTOS - TRACKING

        #region DATATABLE FOR GRIDVIEW
        public DataTable getPickupCargoGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridPickupCargo.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridPickupCargo.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getBranchAcceptanceGrid()
        {
            DataTable dt = new DataTable();
            foreach (GridViewColumn col in this.gridBranchAcceptance.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();

            foreach (GridRowElement row in this.gridBranchAcceptance.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getBundleGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridBundle.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridBundle.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getUnbundleGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridUnbundle.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridUnbundle.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getGatewayTransmitalGrid()
        {

            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridGatewayTransmital.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridGatewayTransmital.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {
                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getGatewayOutboundGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridGatewayOutbound.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridGatewayOutbound.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getGatewayInboundGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridGatewayInbound.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridGatewayInbound.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getCargoTranferGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridCargoTransfer.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridCargoTransfer.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getSegregationGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridSegregation.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridSegregation.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getDeliveryStatusGrid()
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridDeliveryStatus.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridDeliveryStatus.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        public DataTable getDailyTripGrid(string _paymentmode)
        {
            DataTable dt = new DataTable();

            foreach (GridViewColumn col in gridDailyTrip.Columns)
            {
                dt.Columns.Add(new DataColumn(col.HeaderText, typeof(string)));
            }

            dt.BeginLoadData();
            foreach (GridRowElement row in this.gridDailyTrip.TableElement.VisualRows)
            {
                if (row is GridDataRowElement)
                {

                    DataRow dRow = dt.NewRow();
                    foreach (GridViewCellInfo cell in row.RowInfo.Cells)
                    {
                        dRow[cell.ColumnInfo.Index] = cell.Value.ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            dt.EndLoadData();
            DataView view = new DataView(dt);
            view.RowFilter = String.Format("PaymentCode = '{0}'", _paymentmode);
            dt = view.ToTable();
            return dt;
        }
        #endregion

        #region GET DATA METHOD REGION
        /// <summary>
        /// PICKUP CARGO
        /// </summary>
        public void getPickupCargoData()
        {

            PickupCargoManifestReport pickup = new PickupCargoManifestReport();
            List<RevenueUnit> _revenueUnit = getRevenueList();

            DataTable dataTable = pickup.getPickUpCargoData(dateTimePicker_PickupCargo.Value);
            DataView view = new DataView(dataTable);
            gridPickupCargo.DataSource = dataTable;

            //AREA
            DataTable table = view.ToTable(true, "Area");
            dropDownPickUpCargo_Area.Items.Clear();
            dropDownPickUpCargo_Area.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownPickUpCargo_Area.Items.Add(x["Area"].ToString());
            }
            dropDownPickUpCargo_Area.SelectedIndex = 0;

            #region PickupCargo Grid Design
            if (gridPickupCargo.DataSource != null)
            {
                List<int> width = pickup.setPickUpCargoWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridPickupCargo.Columns[ctr].IsVisible = false; }
                    gridPickupCargo.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridPickupCargo.ColumnCount; x++)
                {
                    gridPickupCargo.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion PickupCargo Grid Design
        }
        /// <summary>
        /// BRANCH ACCEPTANCE
        /// </summary>
        public void getBrancAcceptanceData()
        {
            BranchAcceptanceReport branchAccept = new BranchAcceptanceReport();
            DataTable dataTable = branchAccept.getBranchAcceptanceData(dateTimePickerBranchAcceptance_Date.Value);
            DataView view = new DataView(dataTable);

            //DRIVER
            DataTable table = view.ToTable(true, "Driver");
            dropDownBranchAcceptance_Driver.Items.Clear();
            dropDownBranchAcceptance_Driver.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownBranchAcceptance_Driver.Items.Add(x["Driver"].ToString());
            }
            dropDownBranchAcceptance_Driver.SelectedIndex = 0;

            //BATCH
            table = view.ToTable(true, "Batch");
            dropDownBranchAcceptance_Batch.Items.Clear();
            dropDownBranchAcceptance_Batch.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownBranchAcceptance_Batch.Items.Add(x["Batch"].ToString());
            }
            dropDownBranchAcceptance_Batch.SelectedIndex = 0;

            dropDownBranchAcceptance_BCO_BSO.Items.Clear();
            dropDownBranchAcceptance_BCO_BSO.Items.Add("All");

            gridBranchAcceptance.DataSource = dataTable;

            #region Branch Acceptance Grid Design
            if (gridBranchAcceptance.DataSource != null)
            {
                List<int> width = branchAccept.setBranchAcceptanceWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridBranchAcceptance.Columns[ctr].IsVisible = false; }
                    gridBranchAcceptance.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridBranchAcceptance.ColumnCount; x++)
                {
                    gridBranchAcceptance.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion Branch Acceptance Grid Design
        }
        /// <summary>
        /// BUNDLE
        /// </summary>
        private void getBundleData()
        {
            BundleReport bundle = new BundleReport();
            DataTable dataTable = bundle.getBundleData(dateTimeBundle_Date.Value);

            DataView view = new DataView(dataTable);
            //SACK NO
            DataTable table = view.ToTable(true, "SackNo");
            dropDownBundle_SackNo.Items.Clear();
            dropDownBundle_SackNo.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["SackNo"].ToString().Trim() != null)
                {
                    dropDownBundle_SackNo.Items.Add(x["SackNo"].ToString());
                }
            }
            dropDownBundle_SackNo.SelectedIndex = 0;

            //DESTINATION
            table = view.ToTable(true, "Destination");
            dropDownBundle_Destination.Items.Clear();
            dropDownBundle_Destination.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Destination"].ToString().Trim() != null)
                {
                    dropDownBundle_Destination.Items.Add(x["Destination"].ToString());
                }
            }
            dropDownBundle_Destination.SelectedIndex = 0;

            dropDownBundle_BCO_BSO.Items.Clear();
            dropDownBundle_BCO_BSO.Items.Add("All");

            gridBundle.DataSource = dataTable;
            gridBundle.Columns["SackNo"].IsVisible = false;

            #region Bundle Grid Design
            if (gridBundle.DataSource != null)
            {
                List<int> width = bundle.setBundleWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridBundle.Columns[ctr].IsVisible = false; }
                    gridBundle.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridBundle.ColumnCount; x++)
                {
                    gridBundle.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion Bundle Grid Design
        }
        /// <summary>
        /// UNBUNDLE
        /// </summary>
        private void getUnbundle()
        {
            UnbundleReport bundle = new UnbundleReport();

            DataTable dataTable = bundle.getBundleData(dateTimeUnbunde_Date.Value);
            DataView view = new DataView(dataTable);

            //BRANCH
            DataTable table = view.ToTable(true, "Branch");
            dropDownUnbundle_BCO.Items.Clear();
            dropDownUnbundle_BCO.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Branch"].ToString().Trim() != "")
                {
                    dropDownUnbundle_BCO.Items.Add(x["Branch"].ToString());
                }
            }
            dropDownUnbundle_BCO.SelectedIndex = 0;

            //SACK NO
            table = view.ToTable(true, "Sack No");
            dropDownUnbundle_SackNo.Items.Clear();
            dropDownUnbundle_SackNo.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Sack No"].ToString().Trim() != "")
                {
                    dropDownUnbundle_SackNo.Items.Add(x["Sack No"].ToString());
                }
            }
            dropDownUnbundle_SackNo.SelectedIndex = 0;

            gridUnbundle.DataSource = dataTable;

            #region Bundle Grid Design
            if (gridUnbundle.DataSource != null)
            {
                List<int> width = bundle.setBundleWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridUnbundle.Columns[ctr].IsVisible = false; }
                    gridUnbundle.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridUnbundle.ColumnCount; x++)
                {
                    gridUnbundle.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion Bundle Grid Design
        }
        /// <summary>
        /// GATEWAY TRANSMITAL
        /// </summary>
        private void getGatewayTransmitalData()
        {

            GatewayTransmitalReport gatewayTransmitalre = new GatewayTransmitalReport();

            DataTable dataTable = gatewayTransmitalre.getData(dateTimeGatewayTransmital_Date.Value);
            gridGatewayTransmital.DataSource = dataTable;

            DataView view = new DataView(dataTable);

            //GATEWAY
            DataTable table = view.ToTable(true, "Gateway");
            dropDownGatewayTransmital_Gateway.Items.Clear();
            dropDownGatewayTransmital_Gateway.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Gateway"].ToString() != null)
                {
                    dropDownGatewayTransmital_Gateway.Items.Add(x["Gateway"].ToString());
                }
            }
            dropDownGatewayTransmital_Gateway.SelectedIndex = 0;

            //DESTINATION
            table = view.ToTable(true, "Destination");
            dropDownGatewayTransmital_Destination.Items.Clear();
            dropDownGatewayTransmital_Destination.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Destination"].ToString() != null)
                {
                    dropDownGatewayTransmital_Destination.Items.Add(x["Destination"].ToString());
                }
            }
            dropDownGatewayTransmital_Destination.SelectedIndex = 0;

            //BATCH
            table = view.ToTable(true, "Batch");
            dropDownGatewayTransmital_Batch.Items.Clear();
            dropDownGatewayTransmital_Batch.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Batch"].ToString() != null)
                {
                    dropDownGatewayTransmital_Batch.Items.Add(x["Batch"].ToString());
                }
            }
            dropDownGatewayTransmital_Batch.SelectedIndex = 0;


            if (gridGatewayTransmital.DataSource != null)
            {
                List<int> width = gatewayTransmitalre.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    gridGatewayTransmital.Columns[ctr].Width = x;
                    if (x == 0) { gridGatewayTransmital.Columns[ctr].IsVisible = false; }
                    ctr++;
                }
                for (int x = 0; x < gridGatewayTransmital.ColumnCount; x++)
                {
                    gridGatewayTransmital.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }

            }
        }
        /// <summary>
        /// GATEWAY OUTBOUND
        /// </summary>
        private void getGatewayOutBoundData()
        {
            GatewayOutboundReport gatewayOutbound = new GatewayOutboundReport();

            DataTable dataTable = gatewayOutbound.getData(dateTimeGatewayOutbound_Date.Value);

            DataView view = new DataView(dataTable);

            //GATEWAY
            DataTable table = view.ToTable(true, "Gateway");
            dropDownGatewayOutbound_Gateway.Items.Clear();
            dropDownGatewayOutbound_Gateway.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Gateway"].ToString() != null)
                {
                    dropDownGatewayOutbound_Gateway.Items.Add(x["Gateway"].ToString());
                }
            }
            dropDownGatewayOutbound_Gateway.SelectedIndex = 0;

            //BATCH
            table = view.ToTable(true, "Batch");
            dropDownGatewayOutbound_Batch.Items.Clear();
            dropDownGatewayOutbound_Batch.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (!x["Batch"].ToString().Equals(""))
                {
                    dropDownGatewayOutbound_Batch.Items.Add(x["Batch"].ToString());
                }
            }
            dropDownGatewayOutbound_Batch.SelectedIndex = 0;


            gridGatewayOutbound.DataSource = dataTable;

            if (gridGatewayOutbound.DataSource != null)
            {
                List<int> width = gatewayOutbound.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridGatewayOutbound.Columns[ctr].IsVisible = false; }
                    gridGatewayOutbound.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridGatewayOutbound.ColumnCount; x++)
                {
                    gridGatewayOutbound.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
        }
        /// <summary>
        /// GATEWAY INBOUND
        /// </summary>
        private void getGatewayInBoundData()
        {
            GatewayInboundReport gatewayInbound = new GatewayInboundReport();

            DataTable dataTable = gatewayInbound.getData(dateTimePickerGatewayInbound_Date.Value);

            DataView view = new DataView(dataTable);

            DataTable table = view.ToTable(true, "Gateway");
            dropDownGatewayInbound_Gateway.Items.Clear();
            dropDownGatewayInbound_Gateway.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownGatewayInbound_Gateway.Items.Add(x["Gateway"].ToString());
            }
            dropDownGatewayInbound_Gateway.SelectedIndex = 0;

            table = view.ToTable(true, "Origin");
            dropDownGatewayInbound_Origin.Items.Clear();
            dropDownGatewayInbound_Origin.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownGatewayInbound_Origin.Items.Add(x["Origin"].ToString());
            }
            dropDownGatewayInbound_Origin.SelectedIndex = 0;

            table = view.ToTable(true, "Commodity Type");
            dropDownGatewayInbound_Commodity.Items.Clear();
            dropDownGatewayInbound_Commodity.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                dropDownGatewayInbound_Commodity.Items.Add(x["Commodity Type"].ToString());
            }
            dropDownGatewayInbound_Commodity.SelectedIndex = 0;

            gridGatewayInbound.DataSource = dataTable;

            if (gridGatewayInbound.DataSource != null)
            {
                List<int> width = gatewayInbound.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridGatewayInbound.Columns[ctr].IsVisible = false; }
                    gridGatewayInbound.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridGatewayInbound.ColumnCount; x++)
                {
                    gridGatewayInbound.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
        }
        /// <summary>
        /// CARGO TRANSFER
        /// </summary>
        /// 
        private void getCargoTransferData()
        {
            CargoTransferReport cargoTransfer = new CargoTransferReport();

            DataTable dataTable = cargoTransfer.getData(dateTimeCargoTransfer_Date.Value);

            DataView view = new DataView(dataTable);

            //ORIGIN
            DataTable table = view.ToTable(true, "Origin");
            dropDownCargoTransfer_City.Items.Clear();
            dropDownCargoTransfer_City.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Origin"].ToString().Trim() != "")
                {
                    dropDownCargoTransfer_City.Items.Add(x["Origin"].ToString());
                }
            }
            dropDownCargoTransfer_City.SelectedIndex = 0;

            table = view.ToTable(true, "Destination");
            dropDownCargoTransfer_Destination.Items.Clear();
            dropDownCargoTransfer_Destination.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Destination"].ToString() != "")
                {
                    dropDownCargoTransfer_Destination.Items.Add(x["Destination"].ToString());
                }
            }
            dropDownCargoTransfer_Destination.SelectedIndex = 0;


            gridCargoTransfer.DataSource = dataTable;

            #region Cargo Transfer Design
            if (gridCargoTransfer.DataSource != null)
            {
                List<int> width = cargoTransfer.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridCargoTransfer.Columns[ctr].IsVisible = false; }
                    gridCargoTransfer.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridCargoTransfer.ColumnCount; x++)
                {
                    gridCargoTransfer.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion Cargo Transfer Grid Design
        }
        /// <summary>
        /// SEGREGATION
        /// </summary>
        private void getSegregationData()
        {

            SegregationReport segregation = new SegregationReport();
            DataTable dataTable = segregation.getData(dateTimeSegregation_Date.Value);

            DataView view = new DataView(dataTable);

            //BRANCH
            DataTable table = view.ToTable(true, "Branch Corp Office");
            dropDownSegregation_BCO.Items.Clear();
            dropDownSegregation_BCO.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Branch Corp Office"].ToString().Trim() != "")
                {
                    dropDownSegregation_BCO.Items.Add(x["Branch Corp Office"].ToString());
                }
            }
            dropDownSegregation_BCO.SelectedIndex = 0;

            //DRIVER
            table = view.ToTable(true, "Driver");
            dropDownSegregation_Driver.Items.Clear();
            dropDownSegregation_Driver.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Driver"].ToString().Trim() != "")
                {
                    dropDownSegregation_Driver.Items.Add(x["Driver"].ToString());
                }
            }
            dropDownSegregation_Driver.SelectedIndex = 0;

            //PLATE NO
            table = view.ToTable(true, "Plate #");
            dropDownSegregation_PlateNo.Items.Clear();
            dropDownSegregation_PlateNo.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Plate #"].ToString().Trim() != "")
                {
                    dropDownSegregation_PlateNo.Items.Add(x["Plate #"].ToString());
                }
            }
            dropDownSegregation_PlateNo.SelectedIndex = 0;

            //BATCH
            table = view.ToTable(true, "Batch");
            dropDownSegregation_Batch.Items.Clear();
            dropDownSegregation_Batch.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Batch"].ToString().Trim() != "")
                {
                    dropDownSegregation_Batch.Items.Add(x["Batch"].ToString());
                }
            }
            dropDownSegregation_Batch.SelectedIndex = 0;


            gridSegregation.DataSource = dataTable;
            #region
            if (gridSegregation.DataSource != null)
            {
                List<int> width = segregation.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridSegregation.Columns[ctr].IsVisible = false; }
                    gridSegregation.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridSegregation.ColumnCount; x++)
                {
                    gridSegregation.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion
        }
        /// <summary>
        /// DAILY TRIP
        /// </summary>
        private void getDailyTripData()
        {
            DailyTripReport dailyTrip = new DailyTripReport();
            DataTable dataTable = dailyTrip.getData(dateTimeDailyTrip_Date.Value);


            ////AREA
            DataView view = new DataView(dataTable);
            DataTable table = view.ToTable(true, "Area");
            dropDownDailyTrip_Area.Items.Clear();
            dropDownDailyTrip_Area.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Area"].ToString().Trim() != "")
                {
                    dropDownDailyTrip_Area.Items.Add(x["Area"].ToString());
                }
            }
            dropDownDailyTrip_Area.SelectedIndex = 0;

            //DRIVER
            table = view.ToTable(true, "Driver");
            dropDownDailyTrip_Driver.Items.Clear();
            dropDownDailyTrip_Driver.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Driver"].ToString().Trim() != "")
                {
                    dropDownDailyTrip_Driver.Items.Add(x["Driver"].ToString());
                }
            }
            dropDownDailyTrip_Driver.SelectedIndex = 0;

            //PAYMENT MODE
            table = view.ToTable(true, "Payment Mode");
            dropDownDailyTrip_PaymentMode.Items.Clear();
            dropDownDailyTrip_PaymentMode.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Payment Mode"].ToString().Trim() != "")
                {
                    dropDownDailyTrip_PaymentMode.Items.Add(x["Payment Mode"].ToString());
                }
            }
            dropDownDailyTrip_PaymentMode.SelectedIndex = 0;

            //BCO
            table = view.ToTable(true, "BCO");
            dropDownDailyTrip_BCO.Items.Clear();
            dropDownDailyTrip_BCO.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["BCO"].ToString().Trim() != "")
                {
                    dropDownDailyTrip_BCO.Items.Add(x["BCO"].ToString());
                }
            }
            dropDownDailyTrip_BCO.SelectedIndex = 0;

            gridDailyTrip.DataSource = dataTable;

            #region
            if (gridDailyTrip.DataSource != null)
            {
                List<int> width = dailyTrip.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridDailyTrip.Columns[ctr].IsVisible = false; }
                    gridDailyTrip.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridDailyTrip.ColumnCount; x++)
                {
                    gridDailyTrip.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion
        }
        /// <summary>
        /// HOLD CARGO
        /// </summary>
        private void getHoldCargoData()
        {
            HoldCargoReport holdCargo = new HoldCargoReport();

            DataTable dataTable = holdCargo.getData(dateTimeHoldCargo_FromDate.Value, dateTimeHoldCargo_ToDate.Value);

            //STATUS
            DataView view = new DataView(dataTable);
            DataTable table = view.ToTable(true, "Status");
            dropDownHoldCargo_Status.Items.Clear();
            dropDownHoldCargo_Status.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Status"].ToString() != null)
                {
                    dropDownHoldCargo_Status.Items.Add(x["Status"].ToString());
                }
            }
            dropDownHoldCargo_Status.SelectedIndex = 0;

            gridHoldCargo.DataSource = dataTable;

            #region Hold Cargo Grid Design
            if (gridHoldCargo.DataSource != null)
            {
                List<int> width = holdCargo.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridHoldCargo.Columns[ctr].IsVisible = false; }
                    gridHoldCargo.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridHoldCargo.ColumnCount; x++)
                {
                    gridHoldCargo.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion Hold Cargo Grid Design
        }
        /// <summary>
        /// DELIVERY STATUS
        /// </summary>
        private void getDeliveryStatusData()
        {
            DeliveryStatusReport deliveryStatus = new DeliveryStatusReport();
            DataTable dataTable = deliveryStatus.getData(dateTimeDeliveryStatus_Date.Value);
            ////AREA
            DataView view = new DataView(dataTable);

            DataTable table = view.ToTable(true, "Area");
            dropDownDeliveryStatus_Area.Items.Clear();
            dropDownDeliveryStatus_Area.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Area"].ToString().Trim() != "")
                {
                    dropDownDeliveryStatus_Area.Items.Add(x["Area"].ToString());
                }
            }
            dropDownDeliveryStatus_Area.SelectedIndex = 0;

            //DRIVER
            table = view.ToTable(true, "Driver");
            dropDownDeliveryStatus_Driver.Items.Clear();
            dropDownDeliveryStatus_Driver.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Driver"].ToString().Trim() != "")
                {
                    dropDownDeliveryStatus_Driver.Items.Add(x["Driver"].ToString());
                }
            }
            dropDownDeliveryStatus_Driver.SelectedIndex = 0;

            //STATUS
            table = view.ToTable(true, "Status");
            dropDownDeliveryStatus_Status.Items.Clear();
            dropDownDeliveryStatus_Status.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["Status"].ToString().Trim() != "")
                {
                    dropDownDeliveryStatus_Status.Items.Add(x["Status"].ToString());
                }
            }
            dropDownDeliveryStatus_Status.SelectedIndex = 0;

            //BCO
            table = view.ToTable(true, "BCO");
            dropDownDeliveryStatus_BCO.Items.Clear();
            dropDownDeliveryStatus_BCO.Items.Add("All");
            foreach (DataRow x in table.Rows)
            {
                if (x["BCO"].ToString().Trim() != "")
                {
                    dropDownDeliveryStatus_BCO.Items.Add(x["BCO"].ToString());
                }
            }
            dropDownDeliveryStatus_BCO.SelectedIndex = 0;

            gridDeliveryStatus.DataSource = dataTable;

            #region
            if (gridDeliveryStatus.DataSource != null)
            {
                List<int> width = deliveryStatus.setWidth();
                int ctr = 0;
                foreach (int x in width)
                {
                    if (x == 0) { gridDeliveryStatus.Columns[ctr].IsVisible = false; }
                    gridDeliveryStatus.Columns[ctr].Width = x;
                    ctr++;
                }
                for (int x = 0; x < gridDeliveryStatus.ColumnCount; x++)
                {
                    gridDeliveryStatus.Columns[x].TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
                }
            }
            #endregion
        }
        #endregion

        #region GET LIST FROM TABLE REGION
        public List<RevenueUnit> getRevenueList()
        {
            revenueUnitService = new RevenueUnitBL();
            List<RevenueUnit> _revenueUnit = revenueUnitService.GetAll().Where(x => x.City.BranchCorpOffice.BranchCorpOfficeId == GlobalVars.DeviceBcoId).OrderBy(x => x.RevenueUnitName).ToList();
            return _revenueUnit;
        }
        public List<BranchCorpOffice> getBranchCorpOffice()
        {

            bcoService = new BranchCorpOfficeBL(GlobalVars.UnitOfWork);
            List<BranchCorpOffice> branchCorpOffices = bcoService.FilterActive().OrderBy(x => x.BranchCorpOfficeName).ToList();
            return branchCorpOffices;
        }
        public BindingSource getRevenueUnitType()
        {
            BindingSource bsRevenueUnitType = new BindingSource();
            RevenueUnitTypeBL revenueUnitTypeService = new RevenueUnitTypeBL();
            List<RevenueUnitType> _revenueUnitType = revenueUnitTypeService.GetAll().Where(x => x.RecordStatus == 1 && x.RevenueUnitTypeName != "Area").ToList();
            bsRevenueUnitType.DataSource = _revenueUnitType;
            return bsRevenueUnitType;
        }
        #endregion

        #region GET COLUMN DATA VIEW
        public String get_Column_DataView(DataTable _table, String _column)
        {
            String Column_Name = "";
            DataView view = new DataView(_table);
            DataTable table = view.ToTable(true, _column);

            table = view.ToTable(true, _column);
            foreach (DataRow x in table.Rows)
            {
                if (x[_column].ToString() != null)
                {
                    Column_Name += x[_column].ToString() + ",";
                }
            }
            Column_Name = Column_Name.TrimEnd(',');

            return Column_Name;
        }
        #endregion

        #region TRACKING - BUTTON and DROPDOWN EVENT REGION
        // **** PICK UP CARGO **** //
        private void btnExport_PickupCargo_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getPickupCargoGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimePicker_PickupCargo.Value.ToLongDateString();
            TrackingReportGlobalModel.Area = dropDownPickUpCargo_Area.SelectedItem.ToString();

            TrackingReportGlobalModel.Driver = get_Column_DataView(dataTable, "Driver");
            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");

            TrackingReportGlobalModel.Report = "PickUpCargo";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void btnSearch_PicupCargo_Click(object sender, EventArgs e)
        {
            this.gridPickupCargo.FilterDescriptors.Clear();
            gridPickupCargo.EnableFiltering = true;
            this.gridPickupCargo.ShowFilteringRow = false;
            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();

            String Area = "";
            try
            {
                Area = dropDownPickUpCargo_Area.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Area = "All";
                dropDownPickUpCargo_Area.SelectedText = "All";
            }

            if (Area == "All")
            {
                gridPickupCargo.EnableFiltering = false;
                getPickupCargoData();
            }
            else if (Area != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
            }
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridPickupCargo.FilterDescriptors.Add(compositeFilter);
        }
        private void dateTimePicker_PickupCargo_ValueChanged(object sender, EventArgs e)
        {
            dropDownPickUpCargo_Area.SelectedIndex = 0;
            gridPickupCargo.EnableFiltering = false;
            getPickupCargoData();
        }

        // **** BRANCH ACCEPTANCE **** //
        private void dropDownBranchAcceptance_Branch_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            BranchAcceptanceReport branchAccept = new BranchAcceptanceReport();
            DataTable dataTable = branchAccept.getBranchAcceptanceData(dateTimePickerBranchAcceptance_Date.Value);
            DataView view = new DataView(dataTable);

            if (dropDownBranchAcceptance_Branch.SelectedIndex == 0)
            {
                label100.Text = "BCO:";
                List<BranchCorpOffice> branchCorpOffices = getBranchCorpOffice().OrderBy(x => x.BranchCorpOfficeName).ToList();
                dropDownBranchAcceptance_BCO_BSO.DataSource = branchCorpOffices;
                dropDownBranchAcceptance_BCO_BSO.DisplayMember = "BranchCorpOfficeName";
                dropDownBranchAcceptance_BCO_BSO.ValueMember = "BranchCorpOfficeId";
                dropDownBranchAcceptance_BCO_BSO.SelectedValue = GlobalVars.DeviceBcoId;
                dropDownBranchAcceptance_BCO_BSO.Enabled = false;
            }
            else
            {
                label100.Text = "BSO:";
                dropDownBranchAcceptance_BCO_BSO.Enabled = true;
                //BSO
                DataTable table = view.ToTable(true, "BSO");
                dropDownBranchAcceptance_BCO_BSO.Items.Clear();
                dropDownBranchAcceptance_BCO_BSO.Items.Add("All");
                foreach (DataRow x in table.Rows)
                {
                    dropDownBranchAcceptance_BCO_BSO.Items.Add(x["BSO"].ToString());
                }
                dropDownBranchAcceptance_BCO_BSO.SelectedIndex = 0;
            }
        }
        private void btnBranchAcceptance_Search_Click(object sender, EventArgs e)
        {
            this.gridBranchAcceptance.FilterDescriptors.Clear();
            gridBranchAcceptance.EnableFiltering = true;
            this.gridBranchAcceptance.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();

            String Branch_Area = "";
            String Driver = "";
            String Batch = "";

            try
            {
                Branch_Area = dropDownBranchAcceptance_BCO_BSO.SelectedItem.ToString();
                Driver = dropDownBranchAcceptance_Driver.SelectedItem.ToString();
                Batch = dropDownBranchAcceptance_Batch.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Branch_Area = "All"; dropDownBranchAcceptance_BCO_BSO.SelectedText = "All";
                Driver = "All"; dropDownBranchAcceptance_Driver.SelectedText = "All";
                Batch = "All"; dropDownBranchAcceptance_Batch.SelectedText = "All";
            }
            if (Branch_Area == "All" && Driver == "All" && Batch == "All")
            {
                gridBranchAcceptance.EnableFiltering = false;
                getBrancAcceptanceData();
            }
            if (Branch_Area != null && Driver == "All" && Batch == "All")
            {
                if (dropDownBranchAcceptance_Branch.SelectedIndex == 0)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Branch_Area));
                }
                else
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Branch_Area));
                }
            }
            else if (Branch_Area == "All" && Driver != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Branch_Area == "All" && Driver == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (Branch_Area != null && Driver != null && Batch == "All")
            {
                if (dropDownBranchAcceptance_Branch.SelectedIndex == 0)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Branch_Area));
                }
                else
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Branch_Area));
                }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Branch_Area != null && Driver == "All" && Batch != null)
            {
                if (dropDownBranchAcceptance_Branch.SelectedIndex == 0)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Branch_Area));
                }
                else
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Branch_Area));
                }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (Branch_Area == "All" && Driver != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (Branch_Area != null && Driver != null && Batch != null)
            {
                if (dropDownBranchAcceptance_Branch.SelectedIndex == 0)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Branch_Area));
                }
                else
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Branch_Area));
                }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }

            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridBranchAcceptance.FilterDescriptors.Add(compositeFilter);
        }
        private void btnBranchAcceptance_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getBranchAcceptanceGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeGatewayTransmital_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Branch = get_Column_DataView(dataTable, "BCO");
            TrackingReportGlobalModel.Driver = dropDownBranchAcceptance_Driver.SelectedItem.ToString();
            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");
            TrackingReportGlobalModel.PlateNo = get_Column_DataView(dataTable, "Plate #");
            TrackingReportGlobalModel.Batch = dropDownBranchAcceptance_Batch.SelectedItem.ToString();
            TrackingReportGlobalModel.ScannedBy = UserTxt.Text.Replace("Welcome!", "");

            TrackingReportGlobalModel.Report = "BranchAcceptance";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();

        }
        private void dateTimePickerBranchAcceptance_Date_ValueChanged(object sender, EventArgs e)
        {
            dropDownBranchAcceptance_Branch.SelectedIndex = 0;
            dropDownBranchAcceptance_BCO_BSO.Items.Clear();
            dropDownBranchAcceptance_BCO_BSO.Items.Add("All");
            dropDownBranchAcceptance_Driver.SelectedIndex = 0;
            dropDownBranchAcceptance_Batch.SelectedIndex = 0;
            gridBranchAcceptance.EnableFiltering = false;
            getBrancAcceptanceData();
        }

        // **** BUNDLE **** //
        private void dropDownBundle_Branch_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

            BundleReport bundle = new BundleReport();
            DataTable dataTable = bundle.getBundleData(dateTimeBundle_Date.Value);
            DataView view = new DataView(dataTable);

            if (dropDownBundle_Branch.SelectedIndex == 0)
            {
                label113.Text = "BCO:";
                List<BranchCorpOffice> branchCorpOffices = getBranchCorpOffice().OrderBy(x => x.BranchCorpOfficeName).ToList();
                dropDownBundle_BCO_BSO.DataSource = branchCorpOffices;
                dropDownBundle_BCO_BSO.DisplayMember = "BranchCorpOfficeName";
                dropDownBundle_BCO_BSO.ValueMember = "BranchCorpOfficeId";
                dropDownBundle_BCO_BSO.SelectedValue = GlobalVars.DeviceBcoId;
                dropDownBundle_BCO_BSO.Enabled = false;
            }
            else
            {
                label113.Text = "BSO:";
                dropDownBundle_BCO_BSO.Enabled = true;
                //BSO
                DataTable table = view.ToTable(true, "BSO");
                dropDownBundle_BCO_BSO.Items.Clear();
                dropDownBundle_BCO_BSO.Items.Add("All");
                foreach (DataRow x in table.Rows)
                {
                    dropDownBundle_BCO_BSO.Items.Add(x["BSO"].ToString());
                }
                dropDownBundle_BCO_BSO.SelectedIndex = 0;
            }
        }
        private void btnBundle_Search_Click(object sender, EventArgs e)
        {
            this.gridBundle.FilterDescriptors.Clear();
            gridBundle.EnableFiltering = true;
            this.gridBundle.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Area = "";
            String SackNo = "";
            String Destination = "";
            try
            {
                Area = dropDownBundle_BCO_BSO.SelectedItem.ToString();
                SackNo = dropDownBundle_SackNo.SelectedItem.ToString();
                Destination = dropDownBundle_Destination.SelectedItem.ToString();
            }
            catch
            {
                Area = "All"; dropDownBundle_BCO_BSO.SelectedText = "All";
                SackNo = "All"; dropDownBundle_SackNo.SelectedText = "All";
                Destination = "All"; dropDownBundle_Destination.SelectedText = "All";
            }

            if (Area == "All" && SackNo == "All" && Destination == "All")
            {
                gridBundle.EnableFiltering = false;
                getBundleData();
            }

            if (Area != null && SackNo == "All" && Destination == "All")
            {
                if (dropDownBundle_Branch.SelectedIndex == 0) { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Area)); }
                else { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Area)); }

            }
            else if (Area != null && SackNo != null && Destination == "All")
            {
                if (dropDownBundle_Branch.SelectedIndex == 0) { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Area)); }
                else { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Area)); }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("SackNo", FilterOperator.IsEqualTo, SackNo));
            }
            else if (Area != null && SackNo == "All" && Destination != null)
            {
                if (dropDownBundle_Branch.SelectedIndex == 0) { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Area)); }
                else { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Area)); }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
            }
            else if (Area == "All" && SackNo != null && Destination != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("SackNo", FilterOperator.IsEqualTo, SackNo));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
            }
            else if (Area == "All" && SackNo != null && Destination == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("SackNo", FilterOperator.IsEqualTo, SackNo));
            }
            else if (Area == "All" && SackNo == "All" && Destination != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
            }
            else if (Area != null && SackNo != null && Destination != null)
            {
                if (dropDownBundle_Branch.SelectedIndex == 0) { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, Area)); }
                else { compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BSO", FilterOperator.IsEqualTo, Area)); }
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("SackNo", FilterOperator.IsEqualTo, SackNo));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
            }

            //compositeFilter.FilterDescriptors.Add(new FilterDescriptor("CreatedDate", FilterOperator.IsEqualTo, CreatedDate));
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridBundle.FilterDescriptors.Add(compositeFilter);
        }
        private void btnBundle_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getBundleGrid();
            TrackingReportGlobalModel.table = dataTable;

            TrackingReportGlobalModel.Date = dateTimeBundle_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.SackNo = get_Column_DataView(dataTable, "SackNo");
            TrackingReportGlobalModel.Destination = dropDownBundle_Destination.SelectedItem.ToString();
            TrackingReportGlobalModel.Weight = get_Column_DataView(dataTable, "AGW");
            TrackingReportGlobalModel.Report = "Bundle";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeBundle_Date_ValueChanged(object sender, EventArgs e)
        {
            gridBundle.EnableFiltering = false;
            getBundleData();
        }
        // **** UNBUNDLE **** //
        private void btnUnbundle_Search_Click(object sender, EventArgs e)
        {

            this.gridUnbundle.FilterDescriptors.Clear();
            gridUnbundle.EnableFiltering = true;
            this.gridUnbundle.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Branch = "";
            String SackNo = "";
            try
            {
                Branch = dropDownUnbundle_BCO.SelectedItem.ToString();
                SackNo = dropDownUnbundle_SackNo.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Branch = "All"; dropDownUnbundle_BCO.SelectedText = "All";
                SackNo = "All"; dropDownUnbundle_SackNo.SelectedText = "All";
            }
            if (Branch == "All" && SackNo == "All")
            {
                gridUnbundle.EnableFiltering = false;
                getUnbundle();
            }
            if (Branch != null && SackNo == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch", FilterOperator.IsEqualTo, Branch));
            }
            else if (Branch == "All" && SackNo != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Sack No", FilterOperator.IsEqualTo, SackNo));
            }
            else if (Branch != null && SackNo != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch", FilterOperator.IsEqualTo, Branch));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Sack No", FilterOperator.IsEqualTo, SackNo));
            }
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridUnbundle.FilterDescriptors.Add(compositeFilter);
        }
        private void btnUnbundle_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getUnbundleGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeUnbunde_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.SackNo = dropDownUnbundle_SackNo.SelectedItem.ToString();
            TrackingReportGlobalModel.Origin = get_Column_DataView(dataTable, "Origin");
            TrackingReportGlobalModel.ScannedBy = UserTxt.Text.Replace("Welcome!", "");
            TrackingReportGlobalModel.Report = "Unbundle";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeUnbunde_Date_ValueChanged(object sender, EventArgs e)
        {
            gridUnbundle.EnableFiltering = false;
            getUnbundle();
        }
        // **** GATEWAY TRANSMITAL **** //
        private void btnGatewayTransmital_Search_Click(object sender, EventArgs e)
        {
            this.gridGatewayTransmital.FilterDescriptors.Clear();
            gridGatewayTransmital.EnableFiltering = true;
            this.gridGatewayTransmital.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Gateway = "";
            String Destination = "";
            String Batch = "";
            try
            {
                Gateway = dropDownGatewayTransmital_Gateway.SelectedItem.ToString();
                Destination = dropDownGatewayTransmital_Destination.SelectedItem.ToString();
                Batch = dropDownGatewayTransmital_Batch.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Gateway = "All"; dropDownGatewayTransmital_Gateway.SelectedText = "All";
                Destination = "All"; dropDownGatewayTransmital_Destination.SelectedText = "All";
                Batch = "All"; dropDownGatewayTransmital_Batch.SelectedText = "All";
            }
            String CreatedDate = dateTimeGatewayTransmital_Date.Value.ToShortDateString();

            if (Gateway == "All" && Destination == "All" && Batch == "All")
            {
                gridGatewayTransmital.EnableFiltering = false;
                getGatewayTransmitalData();
            }
            if (txtGatewayTransmital_MAWB.Text != "")
            {
                dropDownGatewayTransmital_Gateway.SelectedIndex = 0;
                dropDownGatewayTransmital_Destination.SelectedIndex = 0;
                dropDownGatewayTransmital_Batch.SelectedIndex = 0;
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("AWB", FilterOperator.IsEqualTo, txtGatewayTransmital_MAWB.Text.Trim().ToString()));
            }
            else
            {
                if (Gateway != null && Destination != null && Batch != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                }
                else if (Gateway != null && Destination == "All" && Batch == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                }
                else if (Gateway == "All" && Destination != null && Batch == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
                }
                else if (Gateway == "All" && Destination == "All" && Batch != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                }
                else if (Gateway != null && Destination != null && Batch == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
                }
                else if (Gateway != null && Destination == "All" && Batch != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                }
                else if (Gateway == "All" && Destination != null && Batch != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, Destination));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                }
            }
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridGatewayTransmital.FilterDescriptors.Add(compositeFilter);
        }
        private void btnGatewayTransmital_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getGatewayTransmitalGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeGatewayTransmital_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Driver = get_Column_DataView(dataTable, "Driver");
            TrackingReportGlobalModel.PlateNo = get_Column_DataView(dataTable, "PlateNo");
            TrackingReportGlobalModel.AirwayBillNo = get_Column_DataView(dataTable, "AWB");
            TrackingReportGlobalModel.Area = dropDownGatewayTransmital_Destination.SelectedItem.ToString();
            TrackingReportGlobalModel.Gateway = dropDownGatewayTransmital_Gateway.SelectedItem.ToString();

            TrackingReportGlobalModel.Report = "GatewayTransmital";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeGatewayTransmital_Date_ValueChanged(object sender, EventArgs e)
        {
            gridGatewayTransmital.EnableFiltering = false;
            getGatewayTransmitalData();
        }
        // **** GATEWAT OUTBOUND **** //
        private void btnGatewayOutbound_Search_Click(object sender, EventArgs e)
        {
            this.gridGatewayOutbound.FilterDescriptors.Clear();
            gridGatewayOutbound.EnableFiltering = true;
            this.gridGatewayOutbound.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Gateway = "";
            String Batch = "";
            try
            {
                Gateway = dropDownGatewayOutbound_Gateway.SelectedItem.ToString();
                Batch = dropDownGatewayOutbound_Batch.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Gateway = "All"; dropDownGatewayOutbound_Gateway.SelectedText = "All";
                Batch = "All"; dropDownGatewayOutbound_Batch.SelectedText = "All";

            }
            if (Gateway == "All" && Batch == "All")
            {
                gridGatewayOutbound.EnableFiltering = false;
                getGatewayOutBoundData();
            }
            if (Gateway != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (Gateway != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
            }
            else if (Gateway == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }

            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridGatewayOutbound.FilterDescriptors.Add(compositeFilter);
        }
        private void btnGatewayOutbound_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getGatewayOutboundGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeGatewayOutbound_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Driver = get_Column_DataView(dataTable, "Driver");
            TrackingReportGlobalModel.PlateNo = get_Column_DataView(dataTable, "Plate #");
            TrackingReportGlobalModel.Gateway = dropDownGatewayOutbound_Gateway.SelectedItem.ToString();
            TrackingReportGlobalModel.Report = "GatewayOutbound";

            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeGatewayOutbound_Date_ValueChanged(object sender, EventArgs e)
        {
            gridGatewayOutbound.EnableFiltering = false;
            getGatewayOutBoundData();
        }
        // **** GATEWAY INBOUND **** //
        private void btnGatewayInbound_Search_Click(object sender, EventArgs e)
        {
            this.gridGatewayInbound.FilterDescriptors.Clear();
            gridGatewayInbound.EnableFiltering = true;
            this.gridGatewayInbound.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Gateway = "";
            String Origin = "";
            String CommodityType = "";
            try
            {
                Gateway = dropDownGatewayInbound_Gateway.SelectedItem.ToString();
                Origin = dropDownGatewayInbound_Origin.SelectedItem.ToString();
                CommodityType = dropDownGatewayInbound_Commodity.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Gateway = "All"; dropDownGatewayInbound_Gateway.SelectedText = "All";
                Origin = "All"; dropDownGatewayInbound_Origin.SelectedText = "All";
                CommodityType = "All"; dropDownGatewayInbound_Commodity.SelectedText = "All";
            }
            if (txtBoxGatewayInbound_MasterAWB.Text != "")
            {
                dropDownGatewayInbound_Gateway.SelectedIndex = 0;
                dropDownGatewayInbound_Origin.SelectedIndex = 0;
                dropDownGatewayInbound_Commodity.SelectedIndex = 0;
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("MAWB", FilterOperator.IsEqualTo, txtBoxGatewayInbound_MasterAWB.Text));
            }
            else
            {
                if (Gateway == "All" && Origin == "All" && CommodityType == "All")
                {
                    gridGatewayInbound.EnableFiltering = false;
                    getGatewayInBoundData();
                }
                else if (Gateway != null && Origin == "All" && CommodityType == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                }
                else if (Gateway == "All" && Origin != null && CommodityType == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, Origin));
                }
                else if (Gateway == "All" && Origin == "All" && CommodityType != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Commodity Type", FilterOperator.IsEqualTo, CommodityType));
                }
                else if (Gateway != null && Origin != null && CommodityType == "All")
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, Origin));
                }
                else if (Gateway != null && Origin == "All" && CommodityType != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Commodity Type", FilterOperator.IsEqualTo, CommodityType));
                }
                else if (Gateway == "All" && Origin != null && CommodityType != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, Origin));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Commodity Type", FilterOperator.IsEqualTo, CommodityType));
                }
                else if (Gateway != null && Origin != null && CommodityType != null)
                {
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Gateway", FilterOperator.IsEqualTo, Gateway));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, Origin));
                    compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Commodity Type", FilterOperator.IsEqualTo, CommodityType));
                }

            }
            //compositeFilter.FilterDescriptors.Add(new FilterDescriptor("CreatedDate", FilterOperator.IsEqualTo, dateTimePickerGatewayInbound_Date.Value.ToShortDateString()));

            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridGatewayInbound.FilterDescriptors.Add(compositeFilter);

        }
        private void btnGatewayInbound_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getGatewayInboundGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimePickerGatewayInbound_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Gateway = dropDownGatewayInbound_Gateway.SelectedItem.ToString();
            TrackingReportGlobalModel.AirwayBillNo = get_Column_DataView(dataTable, "MAWB");
            TrackingReportGlobalModel.FlightNo = get_Column_DataView(dataTable, "Flight #");
            TrackingReportGlobalModel.CommodityType = dropDownGatewayInbound_Commodity.SelectedItem.ToString();
            TrackingReportGlobalModel.Report = "GatewayInbound";
            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimePickerGatewayInbound_Date_ValueChanged(object sender, EventArgs e)
        {
            gridGatewayInbound.EnableFiltering = false;
            getGatewayInBoundData();
        }
        // **** CARGO TRANSFER **** //
        private void dropDownCargoTransfer_Origin_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

            RevenueUnitBL revenueBL = new RevenueUnitBL();

            if (dropDownCargoTransfer_Origin.SelectedItem.ToString().Equals("Branch Corporate Office"))
            {
                CargoTransferReport cargoTransfer = new CargoTransferReport();
                DataTable dataTable = cargoTransfer.getData(dateTimeCargoTransfer_Date.Value);

                DataView view = new DataView(dataTable);

                //ORIGIN
                DataTable table = view.ToTable(true, "BCO");

                dropDownCargoTransfer_City.Items.Clear();
                dropDownCargoTransfer_City.Items.Add("All");
                dropDownCargoTransfer_Destination.Items.Clear();
                dropDownCargoTransfer_Destination.Items.Add("All");

                foreach (DataRow x in table.Rows)
                {
                    if (x["BCO"].ToString().Trim() != "")
                    {
                        if (x["BCO"].ToString() != null)
                        {
                            dropDownCargoTransfer_City.Items.Add(x["BCO"].ToString());
                            dropDownCargoTransfer_Destination.Items.Add(x["BCO"].ToString());
                        }
                    }
                }
                dropDownCargoTransfer_City.SelectedIndex = 0;
                dropDownCargoTransfer_Destination.SelectedIndex = 0;
            }
            else
            {
                List<RevenueUnit> revenueList = revenueBL.GetAll().OrderBy(x => x.City.CityName).ToList();
                dropDownCargoTransfer_City.DataSource = revenueList;
                dropDownCargoTransfer_City.DisplayMember = "RevenueUnitName";
                dropDownCargoTransfer_City.ValueMember = "RevenueUnitId";
                dropDownCargoTransfer_City.SelectedIndex = 0;

                revenueList = revenueBL.GetAll().OrderBy(x => x.City.CityName).ToList();
                dropDownCargoTransfer_Destination.DataSource = revenueList;
                dropDownCargoTransfer_Destination.DisplayMember = "RevenueUnitName";
                dropDownCargoTransfer_Destination.ValueMember = "RevenueUnitId";
                dropDownCargoTransfer_Destination.SelectedIndex = 0;
            }

        }
        private void btnCargoTransfer_Search_Click(object sender, EventArgs e)
        {
            gridCargoTransfer.EnableFiltering = true;
            this.gridCargoTransfer.ShowFilteringRow = false;
            //getCargoTransferData();
            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            string origin = "";
            string destination = "";
            try
            {
                origin = dropDownCargoTransfer_City.SelectedItem.ToString();
                destination = dropDownCargoTransfer_Destination.SelectedItem.ToString();
            }
            catch (Exception)
            {
                origin = "All"; dropDownCargoTransfer_City.SelectedText = "All";
                destination = "All"; dropDownCargoTransfer_Destination.SelectedText = "All";
            }
            if (origin == "All" && destination == "All")
            {
                gridCargoTransfer.EnableFiltering = false;
                getCargoTransferData();
            }
            else if (origin != null && destination == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, origin));
            }
            else if (destination != null && origin == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, destination));
            }
            else if (origin != null && destination != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Destination", FilterOperator.IsEqualTo, destination));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Origin", FilterOperator.IsEqualTo, origin));
            }
            //compositeFilter.FilterDescriptors.Add(new FilterDescriptor("CreatedDate", FilterOperator.IsEqualTo, dateTimeCargoTransfer_Date.Value.ToShortDateString()));
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridCargoTransfer.FilterDescriptors.Add(compositeFilter);
        }
        private void btnCargoTransfer_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getCargoTranferGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeCargoTransfer_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Origin = dropDownCargoTransfer_City.SelectedItem.ToString();
            TrackingReportGlobalModel.Destination = dropDownCargoTransfer_Destination.SelectedItem.ToString();
            TrackingReportGlobalModel.Driver = get_Column_DataView(dataTable, "Driver");
            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");
            TrackingReportGlobalModel.PlateNo = get_Column_DataView(dataTable, "Plate #");
            TrackingReportGlobalModel.Report = "CargoTransfer";
            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeCargoTransfer_Date_ValueChanged(object sender, EventArgs e)
        {
            gridCargoTransfer.EnableFiltering = false;
            getCargoTransferData();
        }

        // **** SEGREGATION **** //
        private void btnSegregation_Search_Click(object sender, EventArgs e)
        {

            this.gridSegregation.FilterDescriptors.Clear();
            gridSegregation.EnableFiltering = true;
            this.gridSegregation.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String BCO = "";
            String Driver = "";
            String PlateNo = "";
            String Batch = "";
            try
            {
                BCO = dropDownSegregation_BCO.SelectedItem.ToString();
                Driver = dropDownSegregation_Driver.SelectedItem.ToString();
                PlateNo = dropDownSegregation_PlateNo.SelectedItem.ToString();
                Batch = dropDownSegregation_Batch.SelectedItem.ToString();
            }
            catch (Exception)
            {
                BCO = "All"; dropDownSegregation_BCO.SelectedText = "All";
                Driver = "All"; dropDownSegregation_Driver.SelectedText = "All";
                PlateNo = "All"; dropDownSegregation_PlateNo.SelectedText = "All";
                Batch = "All"; dropDownSegregation_Batch.SelectedText = "All";
            }
            if (BCO == "All" && Driver == "All" && PlateNo == "All" && Batch == "All")
            {
                gridSegregation.EnableFiltering = false;
                getSegregationData();
            }

            if (BCO != null && Driver == "All" && PlateNo == "All" && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
            }
            else if (BCO == "All" && Driver != null && PlateNo == "All" && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (BCO == "All" && Driver == "All" && PlateNo != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO == "All" && Driver == "All" && PlateNo == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (BCO != null && Driver != null && PlateNo == "All" && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (BCO != null && Driver == "All" && PlateNo != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO != null && Driver == "All" && PlateNo == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (BCO == "All" && Driver != null && PlateNo != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO == "All" && Driver != null && PlateNo == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (BCO == "All" && Driver == "All" && PlateNo != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
            }
            else if (BCO != null && Driver != null && PlateNo != null && Batch == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO != null && Driver != null && PlateNo == "All" && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));

            }
            else if (BCO != null && Driver == "All" && PlateNo != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO == "All" && Driver != null && PlateNo != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }
            else if (BCO != null && Driver != null && PlateNo != null && Batch != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch Corp Office", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Batch", FilterOperator.IsEqualTo, Batch));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Plate # ", FilterOperator.IsEqualTo, PlateNo));
            }

            compositeFilter.FilterDescriptors.Add(new FilterDescriptor("CreatedDate", FilterOperator.IsEqualTo, dateTimeSegregation_Date.Value.ToShortDateString()));
            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridSegregation.FilterDescriptors.Add(compositeFilter);
        }
        private void btnSegregation_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getSegregationGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeSegregation_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Driver = dropDownSegregation_Driver.SelectedItem.ToString();
            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");
            TrackingReportGlobalModel.PlateNo = dropDownSegregation_PlateNo.SelectedItem.ToString();
            TrackingReportGlobalModel.Report = "Segregation";
            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeSegregation_Date_ValueChanged(object sender, EventArgs e)
        {
            gridSegregation.EnableFiltering = false;
            getSegregationData();
        }

        // **** DAILY TRIP **** //
        private void btnDailyTrip_Search_Click(object sender, EventArgs e)
        {

            this.gridDailyTrip.FilterDescriptors.Clear();
            gridDailyTrip.EnableFiltering = true;
            this.gridDailyTrip.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Area = "";
            String Driver = "";
            String PaymentMode = "";
            String BCO = "";

            try
            {
                Area = dropDownDailyTrip_Area.SelectedItem.ToString();
                Driver = dropDownDailyTrip_Driver.SelectedItem.ToString();
                PaymentMode = dropDownDailyTrip_PaymentMode.SelectedItem.ToString();
                BCO = dropDownDailyTrip_BCO.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Area = "All"; dropDownDailyTrip_Area.SelectedText = "All";
                Driver = "All"; dropDownDailyTrip_Driver.SelectedText = "All";
                PaymentMode = "All"; dropDownDailyTrip_PaymentMode.SelectedText = "All";
                BCO = "All"; dropDownDailyTrip_BCO.SelectedText = "All";
            }
            if (Area == "All" && Driver == "All" && PaymentMode == "All" && BCO == "All")
            {
                gridDailyTrip.EnableFiltering = false;
                getDailyTripData();
            }
            else if (Area != null && Driver == "All" && PaymentMode == "All" && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
            }
            else if (Area != null && Driver != null && PaymentMode == "All" && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && PaymentMode != null && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area == "All" && Driver != null && PaymentMode == "All" && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area == "All" && Driver != null && PaymentMode != null && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area == "All" && Driver == "All" && PaymentMode != null && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area != null && Driver != null && PaymentMode != null && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area == "All" && Driver == "All" && PaymentMode == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
            }
            else if (Area == "All" && Driver == "All" && PaymentMode != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area == "All" && Driver != null && PaymentMode == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && PaymentMode == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
            }

            else if (Area == "All" && Driver != null && PaymentMode != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area != null && Driver != null && PaymentMode == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && PaymentMode != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }
            else if (Area != null && Driver != null && PaymentMode != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Payment Mode", FilterOperator.IsEqualTo, PaymentMode));
            }


            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridDailyTrip.FilterDescriptors.Add(compositeFilter);
        }
        private void btnDailyTrip_Print_Click(object sender, EventArgs e)
        {

            DataTable dataTable = getDailyTripGrid("PP");
            TrackingReportGlobalModel.Date = dateTimeDailyTrip_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Driver = dropDownDailyTrip_Driver.SelectedItem.ToString();

            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");
            TrackingReportGlobalModel.PlateNo = "";
            TrackingReportGlobalModel.Area = get_Column_DataView(dataTable, "Area");

            TrackingReportGlobalModel.table = getDailyTripGrid("PP");
            TrackingReportGlobalModel.table2 = getDailyTripGrid("CAS");
            TrackingReportGlobalModel.table3 = getDailyTripGrid("FC");
            TrackingReportGlobalModel.table4 = getDailyTripGrid("CAC");

            TrackingReportGlobalModel.Report = "DailyTrip";
            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeDailyTrip_Date_ValueChanged(object sender, EventArgs e)
        {
            gridDailyTrip.EnableFiltering = false;
            getDailyTripData();
        }
        // **** HOLD CARGO **** //
        private void dropDownHoldCargo_Branch_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            HoldCargoReport report = new HoldCargoReport();
            DataTable dataTable = report.getData(dateTimeHoldCargo_FromDate.Value, dateTimeHoldCargo_ToDate.Value);
            DataView view = new DataView(dataTable);

            if (dropDownHoldCargo_Branch.SelectedItem.ToString().Equals("Branch Corporate Office"))
            {
                label104.Text = "BCO:";
                List<BranchCorpOffice> branchCorpOffices = getBranchCorpOffice().OrderBy(x => x.BranchCorpOfficeName).ToList();
                dropDownHoldCargo_BCO_BSO.DataSource = branchCorpOffices;
                dropDownHoldCargo_BCO_BSO.DisplayMember = "BranchCorpOfficeName";
                dropDownHoldCargo_BCO_BSO.ValueMember = "BranchCorpOfficeId";
                dropDownHoldCargo_BCO_BSO.SelectedValue = GlobalVars.DeviceBcoId;
                dropDownHoldCargo_BCO_BSO.Enabled = false;
            }
            else
            {
                label104.Text = "BSO:";
                dropDownBranchAcceptance_BCO_BSO.Enabled = true;
                //BSO
                DataTable table = view.ToTable(true, "BSO");
                dropDownHoldCargo_BCO_BSO.Items.Clear();
                dropDownHoldCargo_BCO_BSO.Items.Add("All");
                foreach (DataRow x in table.Rows)
                {
                    dropDownHoldCargo_BCO_BSO.Items.Add(x["BSO"].ToString());
                }
                dropDownHoldCargo_BCO_BSO.SelectedIndex = 0;
            }
        }
        private void btnHoldCargo_Search_Click(object sender, EventArgs e)
        {

            this.gridHoldCargo.FilterDescriptors.Clear();
            gridHoldCargo.EnableFiltering = true;
            this.gridHoldCargo.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Branch = "";
            String Status = "";

            try
            {
                Branch = dropDownHoldCargo_BCO_BSO.SelectedItem.ToString();
                Status = dropDownHoldCargo_Status.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Branch = "All"; dropDownHoldCargo_BCO_BSO.SelectedText = "All";
                Status = "All"; dropDownHoldCargo_Status.SelectedText = "All";

            }
            if (Branch == "All" && Status == "All")
            {
                gridHoldCargo.EnableFiltering = false;
                getHoldCargoData();
            }
            if (Branch != null && Status == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch", FilterOperator.IsEqualTo, Branch));
            }
            else if (Branch == "All" && Status != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }
            else if (Branch != null && Status != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Branch", FilterOperator.IsEqualTo, Branch));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }

            compositeFilter.LogicalOperator = FilterLogicalOperator.And;

            this.gridHoldCargo.FilterDescriptors.Add(compositeFilter);
        }
        private void btnHoldCargo_Export_Click(object sender, EventArgs e)
        {
            saveFileDialog2.Filter = "Excel File (*.xlsx)|*.xlsx";
            saveFileDialog2.DefaultExt = "xlsx";
            saveFileDialog2.AddExtension = true;

            saveFileDialog2.FileName = "HoldCargo_(" + DateTime.Now.ToShortDateString().Replace("/", "_") + ").xlsx";
            saveFileDialog2.ShowDialog();
        }
        private void dateTimeHoldCargo_FromDate_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeHoldCargo_ToDate.Value <= dateTimeHoldCargo_FromDate.Value)
            {
                dateTimeHoldCargo_ToDate.Value = dateTimeHoldCargo_FromDate.Value.AddDays(1);
            }

            gridHoldCargo.EnableFiltering = false;
            getHoldCargoData();
        }
        private void dateTimeHoldCargo_ToDate_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeHoldCargo_ToDate.Value <= dateTimeHoldCargo_FromDate.Value)
            {
                dateTimeHoldCargo_FromDate.Value = dateTimeHoldCargo_ToDate.Value.AddDays(-1);
            }
            gridHoldCargo.EnableFiltering = false;
            getHoldCargoData();

        }


        // **** DELIVERY STATUS **** //
        private void btnDeliveryStatus_Search_Click(object sender, EventArgs e)
        {
            this.gridDeliveryStatus.FilterDescriptors.Clear();
            gridDeliveryStatus.EnableFiltering = true;
            this.gridDeliveryStatus.ShowFilteringRow = false;

            CompositeFilterDescriptor compositeFilter = new CompositeFilterDescriptor();
            String Area = "";
            String Driver = "";
            String Status = "";
            String BCO = "";
            try
            {
                Area = dropDownDeliveryStatus_Area.SelectedItem.ToString();
                Driver = dropDownDeliveryStatus_Driver.SelectedItem.ToString();
                Status = dropDownDeliveryStatus_Status.SelectedItem.ToString();
                BCO = dropDownDeliveryStatus_BCO.SelectedItem.ToString();
            }
            catch (Exception)
            {
                Area = "All"; dropDownDeliveryStatus_Area.SelectedText = "All";
                Driver = "All"; dropDownDeliveryStatus_Driver.SelectedText = "All";
                Status = "All"; dropDownDeliveryStatus_Status.SelectedText = "All";
                BCO = "All"; dropDownDeliveryStatus_BCO.SelectedText = "All";
            }
            if (Area == "All" && Driver == "All" && Status == "All" && BCO == "ALL")
            {
                gridDeliveryStatus.EnableFiltering = false;
                getDeliveryStatusData();
            }
            else if (Area != null && Driver == "All" && Status == "All" && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
            }
            else if (Area != null && Driver != null && Status == "All" && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && Status != null && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }
            else if (Area == "All" && Driver != null && Status == "All" && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area == "All" && Driver != null && Status != null && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }
            else if (Area == "All" && Driver == "All" && Status != null && BCO == "ALL")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }
            else if (Area != null && Driver != null && Status != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
            }
            else if (Area == "All" && Driver == "All" && Status == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
            }
            else if (Area == "All" && Driver == "All" && Status != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
            }
            else if (Area == "All" && Driver != null && Status != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver != null && Status != null && BCO == "All")
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver != null && Status == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && Status != null && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Status", FilterOperator.IsEqualTo, Status));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
            }
            else if (Area == "All" && Driver != null && Status == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Driver", FilterOperator.IsEqualTo, Driver));
            }
            else if (Area != null && Driver == "All" && Status == "All" && BCO != null)
            {
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("Area", FilterOperator.IsEqualTo, Area));
                compositeFilter.FilterDescriptors.Add(new FilterDescriptor("BCO", FilterOperator.IsEqualTo, BCO));
            }

            compositeFilter.LogicalOperator = FilterLogicalOperator.And;
            this.gridDeliveryStatus.FilterDescriptors.Add(compositeFilter);
        }
        private void btnDeliveryStatus_Print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = getDeliveryStatusGrid();
            TrackingReportGlobalModel.table = dataTable;
            TrackingReportGlobalModel.Date = dateTimeDeliveryStatus_Date.Value.ToLongDateString();
            TrackingReportGlobalModel.Driver = dropDownDeliveryStatus_Driver.SelectedItem.ToString();
            TrackingReportGlobalModel.Checker = get_Column_DataView(dataTable, "Checker");
            TrackingReportGlobalModel.Report = "DeliveryStatus";
            ReportViewer viewer = new ReportViewer();
            viewer.Show();
        }
        private void dateTimeDeliveryStatus_Date_ValueChanged(object sender, EventArgs e)
        {
            gridDeliveryStatus.EnableFiltering = false;
            getDeliveryStatusData();
        }

        // **** OTHERS **** //
        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {


            string exportFile = saveFileDialog2.FileName; // @"E:\Samples\" + "HoldCargo_" + DateTime.Now + ".xlsx";
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Telerik.WinControls.Export.GridViewSpreadExport exporter = new Telerik.WinControls.Export.GridViewSpreadExport(this.gridHoldCargo);
                Telerik.WinControls.Export.SpreadExportRenderer renderer = new Telerik.WinControls.Export.SpreadExportRenderer();
                exporter.RunExport(ms, renderer);

                using (System.IO.FileStream fileStream = new System.IO.FileStream(exportFile, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(fileStream);
                    MessageBox.Show("Successfully exported!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }
        #endregion

        #region TRACKING = AUTOCOMPLETE Search
        private void dropDownPickUpCargo_Area_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownPickUpCargo_Area.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBranchAcceptance_Branch_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBranchAcceptance_Branch.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBranchAcceptance_BCO_BSO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBranchAcceptance_BCO_BSO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBranchAcceptance_Driver_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBranchAcceptance_Driver.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBranchAcceptance_Batch_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBranchAcceptance_Batch.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBundle_BCO_BSO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBundle_BCO_BSO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBundle_SackNo_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBundle_SackNo.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownBundle_Destination_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownBundle_Destination.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownUnbundle_BCO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownUnbundle_BCO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownUnbundle_SackNo_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownUnbundle_SackNo.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayTransmital_Gateway_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayTransmital_Gateway.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayTransmital_Destination_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayTransmital_Destination.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayTransmital_Batch_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayTransmital_Batch.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void txtGatewayTransmital_MAWB_Enter(object sender, EventArgs e)
        {
            try
            {
                AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();

                DataView view = new DataView(getGatewayTransmitalGrid());
                DataTable table = view.ToTable(true, "MAWB");

                table = view.ToTable(true, "MAWB");
                foreach (DataRow x in table.Rows)
                {
                    if (x["MAWB"].ToString() != null)
                    {
                        namesCollection.Add(x["MAWB"].ToString());
                    }
                }

                txtGatewayTransmital_MAWB.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtGatewayTransmital_MAWB.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtGatewayTransmital_MAWB.AutoCompleteCustomSource = namesCollection;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayOutbound_BCO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayOutbound_BCO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayOutbound_Gateway_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayOutbound_Gateway.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayOutbound_Batch_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayOutbound_Batch.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayInbound_Gateway_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayInbound_Gateway.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayInbound_Origin_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayInbound_Origin.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownGatewayInbound_Commodity_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownGatewayInbound_Commodity.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void txtBoxGatewayInbound_MasterAWB_Enter(object sender, EventArgs e)
        {
            try
            {
                AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();

                DataView view = new DataView(getGatewayInboundGrid());
                DataTable table = view.ToTable(true, "MAWB");

                table = view.ToTable(true, "MAWB");
                foreach (DataRow x in table.Rows)
                {
                    if (x["MAWB"].ToString() != null)
                    {
                        namesCollection.Add(x["MAWB"].ToString());
                    }
                }

                txtBoxGatewayInbound_MasterAWB.AutoCompleteMode = AutoCompleteMode.Suggest;
                txtBoxGatewayInbound_MasterAWB.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtBoxGatewayInbound_MasterAWB.AutoCompleteCustomSource = namesCollection;
            }
            catch (Exception) { }
        }

        private void dropDownCargoTransfer_Origin_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownCargoTransfer_Origin.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownCargoTransfer_City_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownCargoTransfer_City.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownCargoTransfer_Destination_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownCargoTransfer_Destination.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownSegregation_BCO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownSegregation_BCO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownSegregation_Driver_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownSegregation_Driver.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownSegregation_PlateNo_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownSegregation_PlateNo.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownSegregation_Batch_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownSegregation_Batch.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDailyTrip_BCO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDailyTrip_BCO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDailyTrip_Area_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDailyTrip_Area.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDailyTrip_Driver_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDailyTrip_Driver.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDailyTrip_PaymentMode_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDailyTrip_PaymentMode.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownHoldCargo_BCO_BSO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownHoldCargo_BCO_BSO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownHoldCargo_Status_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownHoldCargo_Status.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDeliveryStatus_BCO_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDeliveryStatus_BCO.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDeliveryStatus_Status_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDeliveryStatus_Status.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDeliveryStatus_Area_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDeliveryStatus_Area.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }

        private void dropDownDeliveryStatus_Driver_Enter(object sender, EventArgs e)
        {
            try
            {
                dropDownDeliveryStatus_Driver.DropDownListElement.AutoCompleteSuggest.SuggestMode = SuggestMode.Contains;
            }
            catch (Exception) { }
        }
        /// <summary>
        /// PAGEVIEW TRACKING SELECTED PAGE CHANGED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageViewTracking_SelectedPageChanged(object sender, EventArgs e)
        {

            switch (pageViewTracking.SelectedPage.Text)
            {
                case "Pickup Cargo":
                    //DATE 
                    dateTimePicker_PickupCargo.Value = DateTime.Now;
                    //CALL METHOD GET DATA - PICK UP CARGO
                    getPickupCargoData();
                    break;
                case "Branch Acceptance":
                    //DATE
                    dateTimePickerBranchAcceptance_Date.Value = DateTime.Now;
                    //SET DROPDOWN BRANCH IF BCO or BSO
                    dropDownBranchAcceptance_Branch.SelectedIndex = 0;
                    //CALL METHOD GET DATA - BRANCH ACCEPTANCE
                    getBrancAcceptanceData();

                    break;
                case "Bundle":
                    //DATE 
                    dateTimeBundle_Date.Value = DateTime.Now;
                    //
                    dropDownBundle_Branch.SelectedIndex = 0;
                    //CALL METHOD GET DATA - BUNDLE
                    getBundleData();
                    break;
                case "Unbundle":
                    //DATE 
                    dateTimeUnbunde_Date.Value = DateTime.Now;

                    //CALL METHOD GET DATA - UNBUNDLE
                    getUnbundle();

                    break;
                case "Gateway Transmital":
                    //DATE 
                    dateTimeGatewayTransmital_Date.Value = DateTime.Now;
                    //CALL METHOD GET DATA - GATEWAY TRANSMITAL
                    getGatewayTransmitalData();

                    break;
                case "Gateway Outbound":
                    //DATE
                    dateTimeGatewayOutbound_Date.Value = DateTime.Now;
                    //SET DROPDOWN BRANCH BCO 
                    dropDownGatewayOutbound_BCO.DataSource = getBranchCorpOffice();
                    dropDownGatewayOutbound_BCO.DisplayMember = "BranchCorpOfficeName";
                    dropDownGatewayOutbound_BCO.ValueMember = "BranchCorpOfficeId";
                    dropDownGatewayOutbound_BCO.SelectedValue = GlobalVars.DeviceBcoId;
                    dropDownGatewayOutbound_BCO.Enabled = false;
                    //CALL METHOD GET DATA - GATEWAY OUTBOUND
                    getGatewayOutBoundData();

                    break;
                case "Gateway Inbound":
                    //DATE 
                    dateTimePickerGatewayInbound_Date.Value = DateTime.Now;
                    //CALL METHOD GET DATA - GATEWAY INBOUND           
                    getGatewayInBoundData();
                    break;
                case "Cargo Transfer":
                    //DATE 
                    dateTimeCargoTransfer_Date.Value = DateTime.Now;
                    //SET DROPDOWN ORIGIN
                    dropDownCargoTransfer_Origin.DataSource = getRevenueUnitType();
                    dropDownCargoTransfer_Origin.DisplayMember = "RevenueUnitTypeName";
                    dropDownCargoTransfer_Origin.ValueMember = "RevenueUnitTypeId";

                    dropDownCargoTransfer_Origin.Items.Add("Branch Corporate Office");
                    dropDownCargoTransfer_Origin.SelectedValue = "Branch Corporate Office";
                    //CALL METHOD GET DATA - CARGO TRANSFER
                    getCargoTransferData();

                    break;
                case "Segregation":
                    //DATE 
                    dateTimeSegregation_Date.Value = DateTime.Now;
                    //CALL METHOD GET DATA - SEGREGATION
                    getSegregationData();

                    break;
                case "Daily Trip":
                    //DATE
                    dateTimeDailyTrip_Date.Value = DateTime.Now;
                    //CALL METHOD GET DATA - DAILY TRIP
                    getDailyTripData();
                    break;
                case "Hold Cargo":
                    dateTimeHoldCargo_FromDate.Value = DateTime.Now;
                    dateTimeHoldCargo_ToDate.Value = DateTime.Now.AddDays(30);

                    dropDownHoldCargo_Branch.DataSource = getRevenueUnitType();
                    dropDownHoldCargo_Branch.DisplayMember = "RevenueUnitTypeName";
                    dropDownHoldCargo_Branch.ValueMember = "RevenueUnitTypeId";
                    dropDownHoldCargo_Branch.Items.Add("Branch Corporate Office");
                    dropDownHoldCargo_Branch.SelectedValue = "Branch Corporate Office";

                    getHoldCargoData();
                    break;
                case "Delivery Status":
                    //DATE
                    dateTimeDeliveryStatus_Date.Value = DateTime.Now;
                    //
                    //List<BranchCorpOffice> branchCorpOffices = getBranchCorpOffice().OrderBy(x => x.BranchCorpOfficeName).ToList();
                    //dropDownDeliveryStatus_BCO.DataSource = branchCorpOffices;
                    //dropDownDeliveryStatus_BCO.DisplayMember = "BranchCorpOfficeName";
                    //dropDownDeliveryStatus_BCO.ValueMember = "BranchCorpOfficeId";
                    //dropDownDeliveryStatus_BCO.SelectedValue = GlobalVars.DeviceBcoId;
                    //dropDownDeliveryStatus_BCO.Enabled = false;
                    //
                    getDeliveryStatusData();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #endregion END MARK SANTOS REGION

    }
}
