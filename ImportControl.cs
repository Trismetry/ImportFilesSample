using System;
using System.Data;
using System.Web.UI;

public enum ImportEntity
{
    Associations = 0,
    Units = 1,
    Accounts = 2,
    AccountContacts = 3,
    Notes = 4,
    FirmWOTypes = 5,
    FirmVIOTypes = 6,
    AssociationVIOTypes = 7,
    AssociationVIOSubTypes = 8,
    AssociationVIOLocations = 9,
    AssociationWOTypes = 10,
    AssociationWOSubTypes = 11,
    AssociationWOLocations = 12,
    Violations = 13,
    WorkOrders = 14,
    ARCPlans = 15,
    TrackedItems = 16,
    Pets = 17,
    FactResponses = 18,
    Vehicles = 19,
    Visitors = 20,
    Transactions = 21,
    Vendors = 22,
    VendorAssociations = 23,
    FirmARCTypes = 24,
    AssociationARCTypes = 25,
    BankValidationXrefs = 26,
    Additional1099Amounts = 27,
    AssociationTransTypes = 28,
    FirmTransTypes = 29,
    BankAccounts = 30
}
public struct DatabaseColumn
{
    public string ColumnName;
    public Type ColumnType;
    public object DefaultValue;

    public DatabaseColumn(string DBColumnName, Type ColumnType, object DefaultValue = null)
    {
        this.ColumnName = DBColumnName;
        this.ColumnType = ColumnType;
        this.DefaultValue = DefaultValue;
    }
}

/// <summary>
/// Base class to user controls on ImportData page
/// </summary>
public abstract class ImportControl : UserControl
{
    public struct ImportColumn
    {
        public string ColumnName;
        public bool Required;
        public bool CustomHandler;
        public Type ColumnType;
        public int? MaxLength;
        public string[] AllowedValues;
        public int? MinValue;
        public int? MaxValue;
        public bool RequireOneOfSelectedColumns;
    }

    protected ImportColumn[][] ImportColumnsInsert = new ImportColumn[Enum.GetValues(typeof(ImportEntity)).Length][];
    protected ImportColumn[][] ImportColumnsUpdate = new ImportColumn[Enum.GetValues(typeof(ImportEntity)).Length][];


    public ImportEntity Entity
    {
        get
        {
            if (Session["ImportEntity"] != null)
                return (ImportEntity)Session["ImportEntity"];
            else
                throw new ApplicationException("ImportEntity was not set");
        }
        set
        {
            Session["ImportEntity"] = value;
        }
    }
    public bool UpdateMode
    {
        get
        {
            if (Session["UpdateMode"] != null)
                return (bool)Session["UpdateMode"];
            else
                throw new ApplicationException("UpdateMode was not set");
        }
        set
        {
            Session["UpdateMode"] = value;
        }
    }
    public DataTable UserData
    {
        get
        {
            if (Session["UserData"] != null)
                return (DataTable)Session["UserData"];
            else
                return null;
        }
        set
        {
            Session["UserData"] = value;
        }
    }
    protected DataTable ImportData
    {
        get
        {
            if (Session["ImportData"] != null)
                return (DataTable)Session["ImportData"];
            else
                return null;
        }
        set
        {
            Session["ImportData"] = value;
        }
    }

    public string ErrorMessage
    {
        get;
        protected set;
    }

    public abstract bool MoveNext();
    public abstract bool Display();

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!IsPostBack)
        {
            Session["ImportEntity"] = null;
            Session["UpdateMode"] = null;
            UserData = null;
        }
    }
}