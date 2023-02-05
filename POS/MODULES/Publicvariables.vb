Imports MySql.Data.MySqlClient
Imports System.Drawing.Printing
Module publicVariables
    Property DisableFormClose As Boolean = False
    Property Messageboolean As Boolean = False
    Property SiNumberToString As String
    Property ThisIsMyInventoryID
    Property Shift As String
    Property BeginningBalance As Decimal
    Property EndingBalance As Decimal
    Property payment As Boolean = False
    Property posandpendingenter As Boolean = False
    Property productprice As Integer
    Property Deleteitem As Boolean = False
    Property getmunicipality
    Property getprovince
    Property Couponisavailable As Boolean
    Property modeoftransaction As Boolean
    Property SyncIsOnProcess As Boolean

    '=============================================================================================================
    'POS INFORMATION
    Property ClientCrewID As String
    Property ClientRole As String
    Property ClientGuid As String
    Property ClientStoreID As String
    Property ClientBrand As String
    Property ClientLocation As String
    Property ClientPostalCode As String
    Property ClientAddress As String
    Property ClientBrgy As String
    Property ClientMunicipality As String
    Property ClientProvince As String
    Property ClientTin As String
    Property ClientTel As String
    Property ClientStorename As String
    Property ClientProductKey As String
    Property ClientMIN As String
    Property ClientMSN As String
    Property ClientPTUN As String

    '==CONNECTION STRINGS
    Property LocalConnectionString As String
    Property CloudConnectionString As String
    Property ValidLocalConnection As Boolean = False
    Property ValidCloudConnection As Boolean = False
    '==SETTINGS
    Property S_SIBeg As Integer
    Property S_ExportPath As String
    Property S_Tax As String
    Property S_ZeroRated_Tax As Double
    Property S_SIFormat As String = ""
    Property S_Terminal_No As String
    Property S_ZeroRated As String
    Property S_Zreading
    Property S_Batter As String
    Property S_Brownie_Mix As String
    Property S_Upgrade_Price As String
    Property S_Backup_Interval As String
    Property S_Backup_Date As String
    Property S_Logo As String
    Property S_Icon As String
    Property S_Layout As String
    Property S_Print As String
    Property S_Reprint As String
    Property S_Print_XZRead As String
    Property S_Print_Returns As String
    Property S_Print_Sales_Report As String

    Property S_Waffle_Bag As Integer
    Property S_Packets As Integer
    Property S_PrintCount As Integer
    Property S_Dev_Comp_Name As String

    Property S_TrainingMode As Boolean = False
    '=============================================================================================================
    Property AutoInventoryReset As Boolean = False

    'Public messageboxappearance As Boolean = False
    Property table As String
    Property fields As String
    Property value As String
    Property where As String
    '=============================================================================================================

    'Payment Form
    Property TEXTBOXMONEYVALUE As Double
    Property TEXTBOXCHANGEVALUE As Double
    'ModeOfTransactionDetails
    Property TEXTBOXFULLNAMEVALUE As String
    Property TEXTBOXREFERENCEVALUE As String
    Property IfConnectionIsConfigured As Boolean
    'Loading
    Property ValidDatabaseLocalConnection As Boolean = False
    Property DISABLESERVEROTHERSPRODUCT As Boolean = False
    Property DisallowedCharacters As String = "!#$%'~`{}^¨|°¬+[]^¨\/,;=?<>*&+=" & """"
    Property DisallowedCharactersCustom As String = "!#$%'~`{}^¨|°¬+[]^¨\,;=?<>*&+=" & """"
    Property ProductTotalPrice As Double = 0
    Property HASOTHERSLOCALPRODUCT As Boolean = False
    Property HASOTHERSSERVERPRODUCT As Boolean = False
    Property BegBalanceBool As Boolean = False
    Property HASUPDATE As Boolean = False
    Property LOGOUTFROMPOS As Boolean = True
    Property S_OLDGRANDTOTAL As Double = 0
    Property OnlineImage As String = "iVBORw0KGgoAAAANSUhEUgAAACQAAAAkCAYAAADhAJiYAAAACXBIWXMAAAsTAAALEwEAmpwYAAAF8WlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDYgNzkuMTY0NzUzLCAyMDIxLzAyLzE1LTExOjUyOjEzICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuMyAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTAzLTIxVDIxOjI3OjI2KzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wMy0yMVQyMjo1NDozOSswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wMy0yMVQyMjo1NDozOSswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDplYTMxNTQ1OS1jYmE5LThmNDYtYmRjZS05Nzg2MTgwZTNiYTQiIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDo1OGRkZTdhMi1iOTIxLWRlNDAtODNhMi05NWE2YWEwMDQ5NzMiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo2ZWU4Njg2YS1lZDJkLWQ3NDItYjAxYy04NWUyZDU0Njg2YWEiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjZlZTg2ODZhLWVkMmQtZDc0Mi1iMDFjLTg1ZTJkNTQ2ODZhYSIgc3RFdnQ6d2hlbj0iMjAyMi0wMy0yMVQyMToyNzoyNiswODowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIyLjMgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDplYTMxNTQ1OS1jYmE5LThmNDYtYmRjZS05Nzg2MTgwZTNiYTQiIHN0RXZ0OndoZW49IjIwMjItMDMtMjFUMjI6NTQ6MzkrMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMi4zIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz4dgRv3AAACjklEQVRYw82YTUhVQRTHHxLuJKIUyo0YhUKCFUkLlRYRmNrCcKGVglSbXNomQQWxEjQ3EeqiQKHyA0F0IaRlC/UZhSgKGSZGLYKU8AlqWdR/4Fy4PGbOfNyruPi5Ov95Py8z58y9kcSZ5FdgHYyDQdABakEJOA4SQGSvEH8+gH8K/oJl0A4qwZG9EBpnhOLZBC/Alf0i5GcO3NpPQh4L4FqYQhMBhTzGwLkwhGZDEvK4F1QoD9wE1eA+6ALvwVYAKdFKjroKqUgGpeARHX1bqW/gbJhCfkRzLAb9llJ/QNFuCPnJpl5kI1a2m0Ie+WDKQqrIVKgFDIPHoAZcBscsxBospLJNhD5Kgr/AJP3YKQOpi2DVQGiNDkvgTj0ECjRS6WDRYK13YY6OEZDDLHgIzBus0xj2LKtjpA6Cz677KchwHQWHFVLiUPzU5GdUQisBRsQS7R2ZVK5BvlImdBu0gV76AVup7yBNIVWvyf4AB7jGKEZEJrhreQv4ShtaJjWnyVbbdOqrNPlNpCYVa5zR5L64jA7TbtykyL/U5ApdZpmY9jsGUqcl2TSDxus0XEVT/K1ZfFqRHWAyO94rlsuULzR4ShcUtwMuUxEvlEqTX1z6o9QKTjjuqQlF7hOTee4XOq+Y1lu0d2SLzzrspYeaK2+CKEoB25rF0x06caskc0mTyfIuaLo90a14SlEmsyipTwIbTOZ6hBqaTmheIVShyWVIMm+Z+mZToQXmVYlrA7JX7KdMfY8oeGIgNMC0gWkm90BSX8vUvxYFJw2EuHd27j/uk9RXcXckr+gGU3RH0yibmWxUUl/O3a/8heIpdFI/Ep/4ntHR1nVu8V0gRm8Uqz5i9IlQdvRjdKOMr3/zHzlINj2z1tSnAAAAAElFTkSuQmCC"
    Property OfflineImage As String = "iVBORw0KGgoAAAANSUhEUgAAACQAAAAkCAYAAADhAJiYAAAACXBIWXMAAAsTAAALEwEAmpwYAAAF8WlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDYgNzkuMTY0NzUzLCAyMDIxLzAyLzE1LTExOjUyOjEzICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuMyAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTAzLTIxVDIxOjI3OjI2KzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wMy0yMVQyMjo1NDo1MSswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wMy0yMVQyMjo1NDo1MSswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDphNGFmYjViNy1kZjA5LTAwNDgtYWRhMC1iNjZhNTQ0ZjI0YTEiIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDo2MmU0OTZmMC03NDBjLWZhNDUtYjJhYi1iMmIwMzAzYjcwMzIiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDoyZTMzYTI4OC02ZTg2LWNmNGYtYjc5ZC02MmZjNTVlYzMwNzAiPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjJlMzNhMjg4LTZlODYtY2Y0Zi1iNzlkLTYyZmM1NWVjMzA3MCIgc3RFdnQ6d2hlbj0iMjAyMi0wMy0yMVQyMToyNzoyNiswODowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIyLjMgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDphNGFmYjViNy1kZjA5LTAwNDgtYWRhMC1iNjZhNTQ0ZjI0YTEiIHN0RXZ0OndoZW49IjIwMjItMDMtMjFUMjI6NTQ6NTErMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMi4zIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz7RQ7CRAAACjUlEQVRYw82YTUhVQRTHH+I2RMyg3IiSGBhYYQSmtBCh/FgYLfpSiHSjS9skpCBqgtpGolooGKiVCJGLoExbaM9IRFFIsShqEaREL1BLQ/8D58LlMXPm417Fxc/V+c/7eZk5Z+6NLOfnvwK/wTh4Dh6CBlABMkECiOwV4s802FbwH3wGD0AVOLgXQuOMUDxrYACU7xchP3Ogej8JeSyAq2EKTQQU8hgFeWEIzYYk5HE7qFABuAnqQCvoAx/AegAp0UoOuwqpSAWXQBcdfVup7+BUmEJ+RHMsA0OWUlugdDeE/ORSL7IRu7ybQh6F4J2FVKmpUAcYAd2gHlwARyzEmiykck2EPkqCf8Ek/ViOgVQRWDEQWqXDErhTvwDnNVIZYNFgrfdhjo6X4DSzYDKYN1inOexZdoeRSgKfXPdTkOH6GqQopMSh+KXJz6iEvgQYEcu0d2RSZw3yVTKhGnAPPKUfsJX6AdIVUo2a7E+QyDVGMSKOgVuWt4BvtKFlUnOabJ1Np75Ik99EalKxxklN7qvL6DDtxi2K/KAmV+Iyy8S03zSQOiHJphs0XqfhKpriP83iU4rsMJPZ9F6xXKZ8icFTOqe4HXCZynihNJr84tIfpVZw1HFPTShyS0ym3y90RjGt12nvyBafddhLdzVX3gRRdAhsaBbPcOjEnZJMsSZz3Lug6fbEY8VTijKZRUn9AfCHyVyLUEPTCc0rhCo1uWxJ5i1T324qtMC8KnFtQPaK3cPUPxEF9w2Ehpk2MMXk2iT1DUz9G1GQZSDEvbNz//EzSf0N7o7kFV1nimo1jbKdyUYl9Ve4+5W/UDyFR9SPxCe+Xjraus4tvgvE6I1ixUeMPhHKjn6MbpTx9WM7y1tE2/jdam4AAAAASUVORK5CYII="
    Property OnlineOffline As Boolean = False
    Property S_SI_NUMBER As Integer = 0
    Property S_TRANSACTION_NUMBER As Integer = 0
    Property RECEIPTLINECOUNT As Integer = 0
    'Public ZXRECEIPTLINECOUNT As Integer = 0
    Property XREADORZREAD As String
    Property INVENTORY_DATATABLE As New DataTable
    '################################################################ Updates Variables
    Property UPDATE_WORKER_CANCEL As Boolean = False
    Property UPDATE_ROW_COUNT As Integer = 0
    Property UPDATE_PRODUCTS_DATATABLE As New DataTable
    Property UPDATE_CATEGORY_DATATABLE As New DataTable
    Property UPDATE_FORMULA_DATATABLE As New DataTable
    Property UPDATE_INVENTORY_DATATABLE As New DataTable
    Property UPDATE_COUPONS_DATATABLE As New DataTable
    Property UPDATE_PARTNERS_DATATABLE As New DataTable
    Property UPDATE_PRICE_CHANGE_DATATABLE As New DataTable
    Property UPDATE_PRICE_CHANGE_BOOL As Boolean = False
    Property UPDATE_COUPON_APPROVAL_DATATABLE As New DataTable
    Property UPDATE_COUPON_APPROVAL_BOOL As Boolean = False
    Property UPDATE_CUSTOM_PROD_APP_DATATABLE As New DataTable
    Property UPDATE_CUSTOM_PROD_APP_BOOL As Boolean = False
    Property PROMPT_MESSAGE_DATATABLE As New DataTable
    Property SELECT_DISCTINCT_PARTNERS_DT As New DataTable
    '################################################################ CUSTOMER INFORMATION
    Property CUST_INFO_FILLED As Boolean = False
    Property CUST_INFO_NAME As String = ""
    Property CUST_INFO_TIN As String = ""
    Property CUST_INFO_ADDRESS As String = ""
    Property CUST_INFO_BUSINESS As String = ""
    '################################################################ EJOURNAL 
    Property EJOURNAL_TotalLines As Integer = 0
    Property EJOURLAN_Content As String = ""
    Property XML_Path As String = ""
End Module
