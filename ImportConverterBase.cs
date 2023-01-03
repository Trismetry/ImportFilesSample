using System;
using System.Data;
using System.Globalization;

/// <summary>
/// Finds value for any of SaveTable's column using column values in UserTable
/// </summary>
public class ImportConverterBase
{
   
    public DateTime? ParseUserDateTime(string UserValue)
    {
        IFormatProvider format = new CultureInfo("en-US");

        if (DateTime.TryParse(s: UserValue, result: out DateTime date, styles: DateTimeStyles.None, provider: format))
            return date;
        else return null;

    }
    public TimeSpan? ParseUserTime(string UserValue)
    {
        if (TimeSpan.TryParse(s: UserValue, result: out TimeSpan time))
            return time;
        else return null;

    }
    public bool ParseUserBoolean(string UserValue)
    {
        bool Result = false;

        if (!string.IsNullOrEmpty(UserValue) && (UserValue.ToUpper().Equals("1", StringComparison.OrdinalIgnoreCase) ||
                UserValue.ToUpper().Equals("Y", StringComparison.OrdinalIgnoreCase)))
            Result = true;

        return Result;
    }
    public static ImportConverterBase Create(ImportEntity Entity, bool UpdateMode)
    {
        ImportConverterBase Converter;

        switch (Entity)
        {
            case ImportEntity.Accounts:
                Converter = new AccountImportConverter();
                break;
            case ImportEntity.AccountContacts:
                Converter = new AccountMailingAddressImportConverter();
                break;
            case ImportEntity.Associations:
                Converter = new AssociationImportConverter();
                break;
            case ImportEntity.WorkOrders:
                Converter = new WorkOrderImportConverter();
                break;
            case ImportEntity.Violations:
                Converter = new ViolationImportConverter();
                break;
            case ImportEntity.ARCPlans:
                Converter = new ARCPlanImportConverter();
                break;
            case ImportEntity.AssociationVIOSubTypes:
                Converter = new AssociationVIOSubTypeImportConverter();
                break;
            case ImportEntity.AssociationWOSubTypes:
                Converter = new AssociationWOSubTypeImportConverter();
                break;
            case ImportEntity.Vendors:
                Converter = new VendorImportConverter();
                break;
            case ImportEntity.FirmTransTypes:
                if (!UpdateMode)
                    Converter = new FirmTransTypeImportConverter();
                else
                    Converter = new FirmTransTypeUpdateImportConverter();
                break;
            case ImportEntity.BankAccounts:
                Converter = new BankAccountImportConverter();
                break;
            default:
                Converter = new ImportConverterBase();
                break;
        }

        return Converter;
    }
    public virtual object Convert(DataRow UserTableRow, DataColumn DatabaseColumn, bool isUpdateMode)
    {
        object Result = null;
        try
        {

            if (UserTableRow.Table.Columns.Contains(DatabaseColumn.ColumnName)
                )
            {
                string UserValue = UserTableRow[DatabaseColumn.ColumnName].ToString();

                if (UserValue.ToLower() == "null" && DatabaseColumn.AllowDBNull)
                {
                    return DatabaseColumn.DefaultValue; //  change string value of "null" to CLR null value
                }

                if (DatabaseColumn.DataType == typeof(DateTime))
                {
                    DateTime? dt = ParseUserDateTime(UserValue);
                    if (dt != null) Result = (DateTime)dt;
                }
                else if (DatabaseColumn.DataType == typeof(TimeSpan))
                {
                    TimeSpan? dt = ParseUserTime(UserValue);
                    if (dt != null) Result = (TimeSpan)dt;
                }
                else
                {
                    if (DatabaseColumn.DataType == typeof(bool))
                    {
                        if (string.IsNullOrEmpty(UserValue))
                            Result = DatabaseColumn.DefaultValue;
                        else
                            Result = ParseUserBoolean(UserValue);
                    }
                    else
                    {
                        Result = System.Convert.ChangeType(UserValue, DatabaseColumn.DataType);
                    }
                }

            }
            else if (isUpdateMode && DatabaseColumn.DataType == typeof(bool))
            {

            }
            else
                Result = DatabaseColumn.DefaultValue;

        }
        catch (Exception ex) { }

        return Result;
    }
}