Imports System.Drawing.Printing
Imports MySql.Data.MySqlClient
Public Class xreadCls
    Property xrConnStr As String
    Property Title As String = "XREAD"
    Property xrTerminal As String = S_Terminal_No
    Property xrGrossSales As Double
    Property xrLessVatVE As Double
    Property xrLessVatDiplomat As Double
    Property xrLessVatOther As Double
    Property xrAddVat As Double
    Property xrDailySales As Double
    '--------------------------------------------------------
    Property xrVatAmount As Double
    Property xrGovTax As Double
    Property xrVatableSales As Double
    Property xrZeroRatedSales As Double
    Property xrVatExempt As Double 'Vat Exempt Sales
    Property xrLessDiscVE As Double 'Coupon Total
    Property xrNetSales As Double 'Amount Due
    '-------------------------CASH--------------------------------
    Property xrCashTotal As Double
    Property xrCreditCard As Double
    Property xrDebitCard As Double
    Property xrMisc As Double
    Property xrGC As Double
    Property xrGCUsed As Double
    Property xrAR As Double
    Property xrTotalExpenses As Double
    Property xrOthers As Double 'N/A
    Property xrCompBegBalance As String
    Property xrDeposit As Double
    Property xrCashinDrawer As Double
    '-------------------------CASH LESS--------------------------------
    Property xrCashless As Double
    Property xrGcash As Double
    Property xrPaymaya As Double
    Property xrShopee As Double
    Property xrFoodPanda As Double
    Property xrGrab As Double
    Property xrCompExpenses As Double
    Property xrCashlessOthers As Double
    '---------------------------------------------------------
    Property xrItemVoid As Double
    Property xrTrxVoid As Double
    Property xrTrxCancel As Double
    Property xrDiplomat As Double
    Property xrTotalDisc As Double
    Property xrSeniorDisc As Double
    Property xrPWDDisc As Double
    Property xrAthleteDisc As Double
    Property xrDiscOthers As Double
    Property xrTakeOutCharge As Double
    Property xrDelCharge As Double
    Property xrReturnEx As Double
    Property xrReturnRef As Double
    Property xrTotalQTYSold As Integer
    Property xrTotalTrxCount As Integer
    Property xrTotalGuest As Integer
    Property xrBegSINo As String
    Property xrEndSINo As String
    Property xrBegTrxNo As String
    Property xrEndTrxNo As String
    Property xrCurrentTotalSales As Double
    Property xrBegBalance As Double

    Property xrCashier As String
    '-------------------------SALES BY CLASS --------------------------------
    Property xrAddOns As Integer
    Property xrFamousBlends As Integer
    Property xrCombo As Integer
    Property xrPerfectC As Integer
    Property xrPremium As Integer
    Property xrSavory As Integer
    Property xrSimplyP As Integer
    '-------------------------CASH BREAKDOWN QTY--------------------------------
    Property xrThousandQty As Integer
    Property xrFiveHundredQty As Integer
    Property xrTwoHundredQty As Integer
    Property xrOneHundredQty As Integer
    Property xrFiftyQty As Integer
    Property xrTwentyQty As Integer
    Property xrTenQty As Integer
    Property xrFiveQty As Integer
    Property xrOneQty As Integer
    Property xrPointTwentyFiveQty As Integer
    Property xrPointFiveQty As Integer
    Property xrCBQTYTotal As Integer
    '-------------------------CASH BREAKDOWN TOTAL --------------------------------
    Property xrThousandTotal As Double
    Property xrFiveHundredTotal As Double
    Property xrTwoHundredTotal As Double
    Property xrOneHundredTotal As Double
    Property xrFiftyTotal As Double
    Property xrTwentyTotal As Double
    Property xrTenTotal As Double
    Property xrFiveTotal As Double
    Property xrOneTotal As Double
    Property xrPointTwentyFiveTotal As Double
    Property xrPointFiveTotal As Double
    Property xrCBGrandTotal As Double
    Property xrXREADDate As String
    Property xrDateFooter As String

    Public Sub FillReportValue(ZReadDate As String)
        Try

            'Total of Product Price(PHP 60.00)
            xrGrossSales = sum("grosssales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' AND active = 1")
            'Zero always
            xrLessVatDiplomat = 0
            'Gross Sales (xrGrossSales):60 / 1.12(Tax) = Vat Exempt (xrVatExempt):53.57 - Grosssales = 6.43(xrLessVatVE)
            xrLessVatVE = sum("lessvat", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Zero always
            xrLessVatOther = 0
            'Gross Sales (xrGrossSales):60 / 1.12 = Vatable Sales (xrVatableSales):53.57 - Grosssales = 6.43(xrVatAmount)
            xrVatAmount = sum("vatpercentage", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            ' Same value as Vat Amount (xrVatAmount)
            xrAddVat = xrVatAmount
            'Total Gift card value used
            xrGCUsed = sum("gc_value", "loc_coupon_data WHERE zreading = " & ZReadDate & " AND coupon_type = 'Fix-1'")
            'Total Value of Gift card
            xrGC = sum("coupon_total", "loc_coupon_data WHERE zreading = " & ZReadDate & " AND coupon_type = 'Fix-1' ")
            'Zero always
            xrGovTax = 0
            'Gross Sales (xrGrossSales):60 / 1.12(Tax) = Vatable Sales (xrVatableSales):53.57
            xrVatableSales = sum("vatablesales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")

            xrZeroRatedSales = sum("zeroratedsales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Gross Sales (xrGrossSales):60 / 1.12(Tax) = Vatable Exempt (xrVatExempt):53.57
            xrVatExempt = sum("vatexemptsales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Total Discounts = Gross Sales ():60 / 1.12 = ():53.57 * 0.20 = 10.71(xrLessDiscVE) |OR| Used Gift Card(xrGCUsed)
            xrLessDiscVE = sum("coupon_total", $"loc_coupon_data WHERE zreading = '{ZReadDate}' AND coupon_type = 'Percentage(w/o vat)' AND status = 1")
            'Gross Sales (xrGrossSales) + Gift Card Used(xrGCSum) - Less Vat(xrLessVatVE) - Vat Amount()xrVatAmount - Discount (xrLessDiscVE)
            xrDailySales = xrGrossSales + xrGCUsed - xrLessVatVE - xrLessDiscVE
            'Gross Sales (xrGrossSales) + Gift Card Used(xrGCSum) - Less Vat(xrLessVatVE) - Vat Amount()xrVatAmount - Discount (xrLessDiscVE)


            xrCashTotal = sum("amountdue", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")

            xrCreditCard = 0
            xrDebitCard = 0
            xrMisc = 0
            xrAR = 0
            xrTotalExpenses = sum("total_amount", $"loc_expense_list WHERE zreading = '{ZReadDate}' AND active = 1")
            xrOthers = 0
            xrCompBegBalance = returnselect("log_description", $"loc_system_logs WHERE log_type IN ('BG-1','BG-2','BG-3','BG-4') AND zreading = '{ZReadDate}' ORDER by log_date_time DESC LIMIT 1")
            xrDeposit = sum("amount", $"loc_deposit WHERE date(transaction_date) = '{ZReadDate}'")

            xrCashless = sum("amountdue", $"loc_daily_transaction WHERE active IN (1,3) AND zreading = '{ZReadDate}' AND transaction_type NOT IN ('Walk-in' , 'Complimentary Expenses')")
            xrGcash = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Gcash' ")
            xrPaymaya = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Paymaya' ")
            xrGrab = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Grab' ")
            xrFoodPanda = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Food Panda' ")
            xrShopee = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Shopee' ")
            xrCompExpenses = sum("amountdue", $"loc_daily_transaction WHERE active = 3 AND zreading = '{ZReadDate}' AND transaction_type = 'Complimentary Expenses' ")
            xrCashlessOthers = sum("amountdue", $"loc_daily_transaction WHERE active = 3 AND zreading = '{ZReadDate}' AND transaction_type = 'Others' ")
            xrReturnEx = sum("quantity", $"loc_daily_transaction_details WHERE active = 2 AND zreading = '{ZReadDate}' ")
            xrReturnRef = sum("total", $"loc_daily_transaction_details WHERE active = 2 AND zreading = '{ZReadDate}' ")
            xrItemVoid = xrReturnEx
            xrTrxVoid = xrReturnEx
            xrTrxCancel = xrReturnEx
            xrDiplomat = 0

            xrTotalDisc = sum("coupon_total", $"loc_coupon_data WHERE zreading = '{ZReadDate}' AND status = 1")
            xrNetSales = xrGrossSales + xrGCUsed - xrLessVatVE - xrTotalDisc

            If xrCompBegBalance = "" Then xrCompBegBalance = 0
            xrCashinDrawer = xrGrossSales - xrCashless - xrLessVatVE - xrTotalDisc - xrTotalExpenses + Double.Parse(xrCompBegBalance)

            xrSeniorDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'Senior Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")

            xrPWDDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'PWD Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")
            xrAthleteDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'Sports Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")
            xrDiscOthers = 0
            xrTakeOutCharge = 0
            xrDelCharge = 0

            xrTotalQTYSold = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND active = 1")
            xrTotalTrxCount = count("transaction_id", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ")
            xrTotalGuest = xrTotalTrxCount

            xrBegSINo = ReturnRowDouble("si_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' LIMIT 1")
            xrEndSINo = ReturnRowDouble("si_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ORDER by transaction_id DESC LIMIT 1")

            '-1 day in zread date
            Dim curDate As DateTime = DateTime.ParseExact(ZReadDate, "yyyy-MM-dd", Nothing)
            Dim formatCurDate As DateTime = curDate.AddDays(-1)
            Dim prevZreadDate = $"{formatCurDate.Year}-{formatCurDate.Month.ToString("D2")}-{formatCurDate.Day.ToString("D2")}"

            If xrBegSINo = 0 Then
                'Re query to get last si number from last zread date
                xrBegSINo = ReturnRowDouble("si_number", $"loc_daily_transaction ORDER by transaction_id DESC LIMIT 1")
                xrEndSINo = xrBegSINo
            End If

            Dim BegSi As Integer = Val(xrBegSINo)
            xrBegSINo = BegSi.ToString(S_SIFormat)

            Dim EndSI As Integer = Val(xrEndSINo)
            xrEndSINo = EndSI.ToString(S_SIFormat)

            xrBegTrxNo = returnselect("transaction_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' LIMIT 1")
            xrEndTrxNo = returnselect("transaction_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ORDER by transaction_id DESC LIMIT 1")

            If xrBegTrxNo = "" Then
                xrBegTrxNo = returnselect("transaction_number", $"loc_daily_transaction ORDER by transaction_id DESC LIMIT 1")
                xrEndTrxNo = xrBegTrxNo
            End If

            'Same value as Gross Sales
            xrCurrentTotalSales = xrGrossSales
            'On load value of Old grand total column in settings table
            xrBegBalance = S_OLDGRANDTOTAL
            xrCashier = returnfullname(ClientCrewID)

            Dim CashBreakDownDatatable As DataTable = ReturnCashBreakdown(ZReadDate, ZReadDate)
            For Each row As DataRow In CashBreakDownDatatable.Rows
                xrThousandQty += row("Thousand")
                xrFiveHundredQty += row("FiveHundreds")
                xrTwoHundredQty += row("TwoHundred")
                xrOneHundredQty += row("OneHundred")
                xrFiftyQty += row("Fifty")
                xrTwentyQty += row("Twenty")
                xrTenQty += row("Ten")
                xrFiveQty += row("Five")
                xrOneQty += row("One")
                xrPointTwentyFiveQty += row("PointTwoFive")
                xrPointFiveQty += row("PointFive")
            Next row

            'With CashBreakDownDatatable
            '    For i As Integer = 0 To .Rows.Count - 1 Step +1
            '        xrThousandQty += CashBreakDownDatatable(i)(19)
            '        xrFiveHundredQty += CashBreakDownDatatable(i)(20)
            '         += CashBreakDownDatatable(i)(21)
            '         += CashBreakDownDatatable(i)(22)
            '        xrFiftyQty += CashBreakDownDatatable(i)(23)
            '        xrTwentyQty += CashBreakDownDatatable(i)(24)
            '        xrTenQty += CashBreakDownDatatable(i)(25)
            '        xrFiveQty += CashBreakDownDatatable(i)(26)
            '        xrOneQty += CashBreakDownDatatable(i)(27)
            '        xrPointTwentyFiveQty += CashBreakDownDatatable(i)(28)
            '        xrPointFiveQty += CashBreakDownDatatable(i)(29)
            '    Next
            'End With

            xrThousandTotal = xrThousandQty * 1000
            xrFiveHundredTotal = xrFiveHundredQty * 500
            xrTwoHundredTotal = xrTwoHundredQty * 200
            xrOneHundredTotal = xrOneHundredQty * 100
            xrFiftyTotal = xrFiftyQty * 50
            xrTwentyTotal = xrTwentyQty * 20
            xrTenTotal = xrTenQty * 10
            xrFiveTotal = xrFiveQty * 5
            xrOneTotal = xrOneQty * 1
            xrPointTwentyFiveTotal = xrPointTwentyFiveQty * 0.25
            xrPointFiveTotal = xrPointFiveQty * 0.05

            xrAddOns = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Add-Ons'")
            xrFamousBlends = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Famous Blends'")
            xrCombo = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Combo'")
            xrPerfectC = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Perfect Combination'")
            xrPremium = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Premium'")
            xrSavory = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Savory'")
            xrSimplyP = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Simply Perfect'")

            xrCBQTYTotal = xrThousandQty + xrFiveHundredQty + xrTwoHundredQty + xrOneHundredQty + xrFiftyQty + xrTenQty + xrFiveQty + xrOneQty + xrPointTwentyFiveQty + xrPointFiveQty
            xrCBGrandTotal = xrThousandTotal + xrFiveHundredTotal + xrTwoHundredTotal + xrOneHundredTotal + xrFiftyTotal + xrTwentyTotal + xrTenTotal + xrFiveTotal + xrOneTotal + xrPointTwentyFiveTotal + xrPointFiveTotal

            xrXREADDate = ZReadDate
            xrDateFooter = ZReadDate & " " & Format(Now(), "HH:mm:ss")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub xReadingBody(sender As Object, e As PrintPageEventArgs)
        Try
            Dim FontDefault As Font
            Dim FontDefaultBold As Font
            Dim BodySpacing As Integer = 0
            Dim FontDefaultLine As Font
            If My.Settings.PrintSize = "57mm" Then
                FontDefaultLine = New Font("Tahoma", 6)
                FontDefault = New Font("Tahoma", 5)
                FontDefaultBold = New Font("Tahoma", 5, FontStyle.Bold)
            Else
                FontDefault = New Font("Tahoma", 6)
                FontDefaultBold = New Font("Tahoma", 6, FontStyle.Bold)
                FontDefaultLine = New Font("Tahoma", 7)
                BodySpacing = 20
            End If

            SimpleTextDisplay(sender, e, Me.Title, FontDefaultBold, 0, RECEIPTLINECOUNT)
            FillEJournalContent(Me.Title, {}, "S", True, True)
            RECEIPTLINECOUNT += 10
            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "DESCRIPTION", FontDefaultBold, 0, RECEIPTLINECOUNT)
            SimpleTextDisplay(sender, e, "QTY/AMOUNT", FontDefaultBold, 130 + BodySpacing, RECEIPTLINECOUNT)
            FillEJournalContent("DESCRIPTION                    QTY/AMOUNT", {"DESCRIPTION", " QTY/AMOUNT"}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 30
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TERMINAL NO.", Me.xrTerminal, FontDefault, 5, 0)
            FillEJournalContent("TERMINAL NO.          " & Me.xrTerminal, {"TERMINAL NO.", Me.xrTerminal}, "LR", True, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GROSS", NUMBERFORMAT(xrGrossSales), FontDefault, 5, 0)
            FillEJournalContent("GROSS         " & NUMBERFORMAT(xrGrossSales), {"GROSS", NUMBERFORMAT(xrGrossSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (VE)", NUMBERFORMAT(xrLessVatVE), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (VE)         " & xrLessVatVE, {"LESS VAT (VE)", NUMBERFORMAT(xrLessVatVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT DIPLOMAT", NUMBERFORMAT(xrLessVatDiplomat), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT DIPLOMAT         " & NUMBERFORMAT(xrLessVatDiplomat), {"LESS VAT DIPLOMAT", NUMBERFORMAT(xrLessVatDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (OTHER)", NUMBERFORMAT(xrLessVatOther), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (OTHER)          " & NUMBERFORMAT(xrLessVatOther), {"LESS VAT (OTHER)", NUMBERFORMAT(xrLessVatOther)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD VAT", NUMBERFORMAT(xrAddVat), FontDefault, 5, 0)
            FillEJournalContent("ADD VAT          " & NUMBERFORMAT(xrAddVat), {"ADD VAT", NUMBERFORMAT(xrAddVat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DAILY SALES", NUMBERFORMAT(xrDailySales), FontDefault, 5, 0)
            FillEJournalContent("DAILY SALES          " & NUMBERFORMAT(xrDailySales), {"DAILY SALES", NUMBERFORMAT(xrDailySales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT AMOUNT", NUMBERFORMAT(xrVatAmount), FontDefault, 5, 0)
            FillEJournalContent("VAT AMOUNT          " & NUMBERFORMAT(xrVatAmount), {"VAT AMOUNT", NUMBERFORMAT(xrVatAmount)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LOCAL GOV'T TAX", NUMBERFORMAT(xrGovTax), FontDefault, 5, 0)
            FillEJournalContent("LOCAL GOV'T TAX          " & NUMBERFORMAT(xrGovTax), {"LOCAL GOV'T TAX", NUMBERFORMAT(xrGovTax)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VATABLE SALES", NUMBERFORMAT(xrVatableSales), FontDefault, 5, 0)
            FillEJournalContent("VATABLE SALES          " & NUMBERFORMAT(xrVatableSales), {"VATABLE SALES", NUMBERFORMAT(xrVatableSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ZERO RATED SALES", NUMBERFORMAT(xrZeroRatedSales), FontDefault, 5, 0)
            FillEJournalContent("ZERO RATED SALES          " & NUMBERFORMAT(xrZeroRatedSales), {"ZERO RATED SALES", NUMBERFORMAT(xrZeroRatedSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT EXEMPT SALES", NUMBERFORMAT(xrVatExempt), FontDefault, 5, 0)
            FillEJournalContent("VAT EXEMPT SALES          " & NUMBERFORMAT(xrVatExempt), {"VAT EXEMPT SALES", NUMBERFORMAT(xrVatExempt)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS DISC (VE)", NUMBERFORMAT(xrLessDiscVE), FontDefault, 5, 0)
            FillEJournalContent("LESS DISC (VE)          " & NUMBERFORMAT(xrLessDiscVE), {"LESS DISC (VE)", NUMBERFORMAT(xrLessDiscVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "NET SALES", NUMBERFORMAT(xrNetSales), FontDefault, 5, 0)
            FillEJournalContent("NET SALES          " & NUMBERFORMAT(xrNetSales), {"NET SALES", NUMBERFORMAT(xrNetSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH TOTAL", NUMBERFORMAT(xrCashTotal), FontDefault, 5, 0)
            FillEJournalContent("CASH TOTAL          " & NUMBERFORMAT(xrCashTotal), {"CASH TOTAL", NUMBERFORMAT(xrCashTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CREDIT CARD", NUMBERFORMAT(xrCreditCard), FontDefault, 5, 0)
            FillEJournalContent("CREDIT CARD          " & NUMBERFORMAT(xrCreditCard), {"CREDIT CARD", NUMBERFORMAT(xrCreditCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEBIT CARD", NUMBERFORMAT(xrDebitCard), FontDefault, 5, 0)
            FillEJournalContent("DEBIT CARD          " & NUMBERFORMAT(xrDebitCard), {"DEBIT CARD", NUMBERFORMAT(xrDebitCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "MISC/CHEQUES", NUMBERFORMAT(xrMisc), FontDefault, 5, 0)
            FillEJournalContent("MISC/CHEQUES          " & NUMBERFORMAT(xrDebitCard), {"MISC/CHEQUES", NUMBERFORMAT(xrMisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD(GC)", NUMBERFORMAT(xrGC), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD(GC)          " & NUMBERFORMAT(xrGC), {"GIFT CARD(GC)", NUMBERFORMAT(xrGC)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD USED", NUMBERFORMAT(xrGCUsed), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD SUM          " & NUMBERFORMAT(xrGCUsed), {"GIFT CARD SUM", NUMBERFORMAT(xrGCUsed)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "A/R", NUMBERFORMAT(xrAR), FontDefault, 5, 0)
            FillEJournalContent("A/R          " & NUMBERFORMAT(xrAR), {"A/R", NUMBERFORMAT(xrAR)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL EXPENSES", NUMBERFORMAT(xrTotalExpenses), FontDefault, 5, 0)
            FillEJournalContent("TOTAL EXPENSES         " & NUMBERFORMAT(xrTotalExpenses), {"TOTAL EXPENSES", NUMBERFORMAT(xrTotalExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", "N/A", FontDefault, 5, 0)
            FillEJournalContent("OTHERS         N/A", {"OTHERS", "N/A"}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEG.BALANCE", NUMBERFORMAT(Double.Parse(xrCompBegBalance)), FontDefault, 5, 0)
            FillEJournalContent("BEG.BALANCE         " & NUMBERFORMAT(Double.Parse(xrCompBegBalance)), {"BEG.BALANCE", NUMBERFORMAT(Double.Parse(xrCompBegBalance))}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            'If ZXBegBalance = 0 Then
            '    RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEG.BALANCE", "0.00", FontDefault, 5, 0)
            '    FillEJournalContent("BEG.BALANCE         0.00", {"BEG.BALANCE", "0.00"}, "LR", False, False)
            '    RECEIPTLINECOUNT += 10
            'Else
            '    RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEG.BALANCE", NUMBERFORMAT(Double.Parse(ZXBegBalance)), FontDefault, 5, 0)
            '    FillEJournalContent("BEG.BALANCE          " & NUMBERFORMAT(Double.Parse(ZXBegBalance)), {"BEG.BALANCE", NUMBERFORMAT(Double.Parse(ZXBegBalance))}, "LR", False, False)
            '    RECEIPTLINECOUNT += 10
            'End If

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEPOSIT", NUMBERFORMAT(xrDeposit), FontDefault, 5, 0)
            FillEJournalContent("DEPOSIT          " & NUMBERFORMAT(xrDeposit), {"DEPOSIT", NUMBERFORMAT(xrDeposit)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH IN DRAWER", NUMBERFORMAT(xrCashinDrawer), FontDefault, 5, 0)
            FillEJournalContent("CASH IN DRAWER          " & NUMBERFORMAT(xrCashinDrawer), {"CASH IN DRAWER", NUMBERFORMAT(xrCashinDrawer)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASHLESS", NUMBERFORMAT(xrCashless), FontDefault, 5, 0)
            FillEJournalContent("CASHLESS          " & NUMBERFORMAT(xrCashless), {"CASHLESS", NUMBERFORMAT(xrCashless)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GCASH", NUMBERFORMAT(xrGcash), FontDefault, 5, 0)
            FillEJournalContent("GCASH          " & NUMBERFORMAT(xrGcash), {"GCASH", NUMBERFORMAT(xrGcash)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PAYMAYA", NUMBERFORMAT(xrPaymaya), FontDefault, 5, 0)
            FillEJournalContent("PAYMAYA          " & NUMBERFORMAT(xrPaymaya), {"PAYMAYA", NUMBERFORMAT(xrPaymaya)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SHOPEE", NUMBERFORMAT(xrShopee), FontDefault, 5, 0)
            FillEJournalContent("SHOPEE          " & NUMBERFORMAT(xrShopee), {"SHOPEE", NUMBERFORMAT(xrShopee)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FOODPANDA", NUMBERFORMAT(xrFoodPanda), FontDefault, 5, 0)
            FillEJournalContent("FOODPANDA          " & NUMBERFORMAT(xrFoodPanda), {"FOODPANDA", NUMBERFORMAT(xrFoodPanda)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAB", NUMBERFORMAT(xrGrab), FontDefault, 5, 0)
            FillEJournalContent("GRAB          " & NUMBERFORMAT(xrGrab), {"GRAB", NUMBERFORMAT(xrGrab)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMPLIMENTARY", NUMBERFORMAT(xrCompExpenses), FontDefault, 5, 0)
            FillEJournalContent("COMPLIMENTARY          " & NUMBERFORMAT(xrCompExpenses), {"COMPLIMENTARY", NUMBERFORMAT(xrCompExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", NUMBERFORMAT(xrCashlessOthers), FontDefault, 5, 0)
            FillEJournalContent("OTHERS          " & NUMBERFORMAT(xrCashlessOthers), {"OTHERS", NUMBERFORMAT(xrCashlessOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ITEM VOID E/C", NUMBERFORMAT(xrItemVoid), FontDefault, 5, 0)
            FillEJournalContent("ITEM VOID E/C          " & NUMBERFORMAT(xrItemVoid), {"ITEM VOID E/C", NUMBERFORMAT(xrItemVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION VOID", NUMBERFORMAT(xrTrxVoid), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION VOID         " & NUMBERFORMAT(xrTrxVoid), {"TRANSACTION VOID", NUMBERFORMAT(xrTrxVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION CANCEL", NUMBERFORMAT(xrTrxCancel), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION CANCEL         " & NUMBERFORMAT(xrTrxCancel), {"TRANSACTION CANCEL", NUMBERFORMAT(xrTrxCancel)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DIMPLOMAT", NUMBERFORMAT(xrDiplomat), FontDefault, 5, 0)
            FillEJournalContent("DIMPLOMAT         " & NUMBERFORMAT(xrDiplomat), {"DIMPLOMAT", NUMBERFORMAT(xrDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL DISCOUNTS", NUMBERFORMAT(xrTotalDisc), FontDefault, 5, 0)
            FillEJournalContent("TOTAL DISCOUNTS         " & NUMBERFORMAT(xrTotalDisc), {"TOTAL DISCOUNTS", NUMBERFORMAT(xrTotalDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - SENIOR CITIZEN", NUMBERFORMAT(xrSeniorDisc), FontDefault, 5, 0)
            FillEJournalContent(" - SENIOR CITIZEN         " & NUMBERFORMAT(xrSeniorDisc), {" - SENIOR CITIZEN", NUMBERFORMAT(xrSeniorDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - PWD", NUMBERFORMAT(xrPWDDisc), FontDefault, 5, 0)
            FillEJournalContent(" - PWD         " & NUMBERFORMAT(xrPWDDisc), {" -PWD", NUMBERFORMAT(xrPWDDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - ATHLETE", NUMBERFORMAT(xrAthleteDisc), FontDefault, 5, 0)
            FillEJournalContent(" - ATHLETE         " & NUMBERFORMAT(xrAthleteDisc), {" - ATHLETE", NUMBERFORMAT(xrAthleteDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - OTHERS", NUMBERFORMAT(xrDiscOthers), FontDefault, 5, 0)
            FillEJournalContent(" - OTHERS         " & NUMBERFORMAT(xrDiscOthers), {" - OTHERS", NUMBERFORMAT(xrDiscOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TAKE OUT CHARGE", NUMBERFORMAT(xrTakeOutCharge), FontDefault, 5, 0)
            FillEJournalContent("TAKE OUT CHARGE         " & NUMBERFORMAT(xrTakeOutCharge), {"TAKE OUT CHARGE", NUMBERFORMAT(xrTakeOutCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DELIVERY CHARGE", NUMBERFORMAT(xrDelCharge), FontDefault, 5, 0)
            FillEJournalContent("DELIVERY CHARGE         " & NUMBERFORMAT(xrDelCharge), {"DELIVERY CHARGE", NUMBERFORMAT(xrDelCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS EXCHANGE", NUMBERFORMAT(xrReturnEx), FontDefault, 5, 0)
            FillEJournalContent("RETURNS EXCHANGE         " & NUMBERFORMAT(xrReturnEx), {"RETURNS EXCHANGE", NUMBERFORMAT(xrReturnEx)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS REFUND", NUMBERFORMAT(xrReturnRef), FontDefault, 5, 0)
            FillEJournalContent("RETURNS REFUND         " & NUMBERFORMAT(xrReturnRef), {"RETURNS REFUND", NUMBERFORMAT(xrReturnRef)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL QTY SOLD", xrTotalQTYSold, FontDefault, 5, 0)
            FillEJournalContent("TOTAL QTY SOLD         " & xrTotalQTYSold, {"TOTAL QTY SOLD", xrTotalQTYSold}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL TRANS. COUNT", xrTotalTrxCount, FontDefault, 5, 0)
            FillEJournalContent("TOTAL TRANS. COUNT         " & xrTotalTrxCount, {"TOTAL TRANS. COUNT", xrTotalTrxCount}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL GUEST", xrTotalGuest, FontDefault, 5, 0)
            FillEJournalContent("TOTAL GUEST         " & xrTotalGuest, {"TOTAL GUEST", xrTotalGuest}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING S.I NO.", xrBegSINo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING S.I NO.         " & xrBegSINo, {"BEGINNING S.I NO.", xrBegSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END S.I NO.", xrEndSINo, FontDefault, 5, 0)
            FillEJournalContent("END S.I NO.         " & xrEndSINo, {"END S.I NO.", xrEndSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING TRANS. NO.", xrBegTrxNo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING TRANS. NO.         " & xrBegTrxNo, {"BEGINNING TRANS. NO.", xrBegTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END TRANS. NO.", xrEndTrxNo, FontDefault, 5, 0)
            FillEJournalContent("END TRANS. NO.         " & xrEndTrxNo, {"END TRANS. NO.", xrEndTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CURRENT TOTAL SALES", NUMBERFORMAT(xrCurrentTotalSales), FontDefault, 5, 0)
            FillEJournalContent("CURRENT TOTAL SALES         " & NUMBERFORMAT(xrCurrentTotalSales), {"CURRENT TOTAL SALES", NUMBERFORMAT(xrCurrentTotalSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING BALANCE", NUMBERFORMAT(xrBegBalance), FontDefault, 5, 0)
            FillEJournalContent("BEGINNING BALANCE         " & NUMBERFORMAT(xrBegBalance), {"BEGINNING BALANCE", NUMBERFORMAT(xrBegBalance)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASHIER", xrCashier, FontDefault, 5, 0)
            FillEJournalContent("CASHIER          " & xrCashier, {"CASHIER", xrCashier}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "SALES BY CLASS", FontDefault, 0, RECEIPTLINECOUNT)
            FillEJournalContent("SALES BY CLASS", {}, "S", False, True)
            RECEIPTLINECOUNT += 30
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD ONS", xrAddOns, FontDefault, 5, 0)
            FillEJournalContent("ADD ONS          " & xrAddOns, {"ADD ONS", xrAddOns}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FAMOUS BLENDS", xrFamousBlends, FontDefault, 5, 0)
            FillEJournalContent("FAMOUS BLENDS          " & xrFamousBlends, {"FAMOUS BLENDS", xrFamousBlends}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMBO", xrCombo, FontDefault, 5, 0)
            FillEJournalContent("COMBO          " & xrCombo, {"COMBO", xrCombo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PERFECT COMBINATION", xrPerfectC, FontDefault, 5, 0)
            FillEJournalContent("PERFECT COMBINATION          " & xrPerfectC, {"PERFECT COMBINATION", xrPerfectC}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PREMIUM LINE", xrPremium, FontDefault, 5, 0)
            FillEJournalContent("PREMIUM LINE          " & xrPremium, {"PREMIUM LINE", xrPremium}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SAVORY", xrSavory, FontDefault, 5, 0)
            FillEJournalContent("SAVORY          " & xrSavory, {"SAVORY", xrSavory}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SIMPY PERFECT", xrSimplyP, FontDefault, 5, 0)
            FillEJournalContent("SIMPY PERFECT          " & xrSimplyP, {"SIMPY PERFECT", xrSimplyP}, "LR", False, False)
            RECEIPTLINECOUNT -= 10

            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "CASH BREAK DOWN", FontDefaultBold, 0, RECEIPTLINECOUNT)
            FillEJournalContent("CASH BREAK DOWN", {}, "S", False, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "Bill type", FontDefault, 0, RECEIPTLINECOUNT)
            SimpleTextDisplay(sender, e, "Quantity", FontDefault, 80, RECEIPTLINECOUNT)
            SimpleTextDisplay(sender, e, "Total", FontDefault, 160, RECEIPTLINECOUNT)
            FillEJournalContent("Bill type           Quantity           Total", {"Bill type", "Quantity", "Total"}, "S3", False, False)
            RECEIPTLINECOUNT += 30
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1000", NUMBERFORMAT(xrThousandTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrThousandQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1000           " & xrThousandQty & "           " & NUMBERFORMAT(xrThousandTotal), {"₱ 1000", xrThousandQty, NUMBERFORMAT(xrThousandTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 500", NUMBERFORMAT(xrFiveHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrFiveHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 500           " & xrFiveHundredQty & "           " & NUMBERFORMAT(xrFiveHundredTotal), {"₱ 500", xrFiveHundredQty, NUMBERFORMAT(xrFiveHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 200", NUMBERFORMAT(xrTwoHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrTwoHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 200           " & xrTwoHundredQty & "           " & NUMBERFORMAT(xrTwoHundredTotal), {"₱ 200", xrTwoHundredQty, NUMBERFORMAT(xrTwoHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 100", NUMBERFORMAT(xrOneHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrOneHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 100           " & xrOneHundredQty & "           " & NUMBERFORMAT(xrOneHundredTotal), {"₱ 100", xrOneHundredQty, NUMBERFORMAT(xrOneHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 50", NUMBERFORMAT(xrFiftyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrFiftyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 50           " & xrFiftyQty & "           " & NUMBERFORMAT(xrFiftyTotal), {"₱ 50", xrFiftyQty, NUMBERFORMAT(xrFiftyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 20", NUMBERFORMAT(xrTwentyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrTwentyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 20           " & xrTwentyQty & "           " & NUMBERFORMAT(xrTwentyTotal), {"₱ 20", xrTwentyQty, NUMBERFORMAT(xrTwentyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 10", NUMBERFORMAT(xrTenTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrTenQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 10           " & xrTenQty & "           " & NUMBERFORMAT(xrTenTotal), {"₱ 10", xrTenQty, NUMBERFORMAT(xrTenTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 5", NUMBERFORMAT(xrFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 5           " & xrFiveQty & "           " & NUMBERFORMAT(xrFiveTotal), {"₱ 5", xrFiveQty, NUMBERFORMAT(xrFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1", NUMBERFORMAT(xrOneTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrOneQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1           " & xrOneQty & "           " & NUMBERFORMAT(xrOneTotal), {"₱ 1", xrOneQty, NUMBERFORMAT(xrOneTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .25", NUMBERFORMAT(xrPointTwentyFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrPointTwentyFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .25           " & xrPointTwentyFiveQty & "           " & NUMBERFORMAT(xrPointTwentyFiveTotal), {"₱ .25", xrPointTwentyFiveQty, NUMBERFORMAT(xrPointTwentyFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .05", NUMBERFORMAT(xrPointFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrPointFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .05           " & xrPointFiveQty & "           " & NUMBERFORMAT(xrPointFiveTotal), {"₱ .05", xrPointFiveQty, NUMBERFORMAT(xrPointFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT -= 10
            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 30

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAND TOTAL", NUMBERFORMAT(xrCBGrandTotal), FontDefaultBold, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", xrCBQTYTotal, FontDefaultBold, 0, 110)
            FillEJournalContent("GRAND TOTAL          " & NUMBERFORMAT(xrCBGrandTotal), {"GRAND TOTAL", NUMBERFORMAT(xrCBGrandTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            CenterTextDisplay(sender, e, xrDateFooter, FontDefault, RECEIPTLINECOUNT)
            FillEJournalContent(xrDateFooter, {}, "C", False, True)

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Function ConnLocal() As MySqlConnection
        Dim Conn As New MySqlConnection
        Conn.ConnectionString = xrConnStr
        Try
            Conn.Open()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return Conn
    End Function
End Class
