Imports System.Drawing.Printing
Imports MySql.Data.MySqlClient
Public Class zreadCls
    Property zrConnStr As String
    Property Title As String = "ZREAD"
    Property zrTerminal As String = S_Terminal_No
    Property zrGrossSales As Double
    Property zrLessVatVE As Double
    Property zrLessVatDiplomat As Double
    Property zrLessVatOther As Double
    Property zrAddVat As Double
    Property zrDailySales As Double
    '--------------------------------------------------------
    Property zrVatAmount As Double
    Property zrGovTax As Double
    Property zrVatableSales As Double
    Property zrZeroRatedSales As Double
    Property zrVatExempt As Double 'Vat Exempt Sales
    Property zrLessDiscVE As Double 'Coupon Total
    Property zrNetSales As Double 'Amount Due
    '-------------------------CASH--------------------------------
    Property zrCashTotal As Double
    Property zrCreditCard As Double
    Property zrDebitCard As Double
    Property zrMisc As Double
    Property zrGC As Double
    Property zrGCUsed As Double
    Property zrAR As Double
    Property zrTotalExpenses As Double
    Property zrOthers As Double 'N/A
    Property zrCompBegBalance As String
    Property zrDeposit As Double
    Property zrCashinDrawer As Double
    '-------------------------CASH LESS--------------------------------
    Property zrCashless As Double
    Property zrGcash As Double
    Property zrPaymaya As Double
    Property zrShopee As Double
    Property zrFoodPanda As Double
    Property zrGrab As Double
    Property zrCompExpenses As Double
    Property zrCashlessOthers As Double
    '---------------------------------------------------------
    Property zrItemVoid As Double
    Property zrTrxVoid As Double
    Property zrTrxCancel As Double
    Property zrDiplomat As Double
    Property zrTotalDisc As Double
    Property zrSeniorDisc As Double
    Property zrPWDDisc As Double
    Property zrAthleteDisc As Double
    Property zrSPDisc As Double
    Property zrDiscOthers As Double
    Property zrTakeOutCharge As Double
    Property zrDelCharge As Double
    Property zrReturnEx As Double
    Property zrReturnRef As Double
    Property zrTotalQTYSold As Integer
    Property zrTotalTrxCount As Integer
    Property zrTotalGuest As Integer
    Property zrBegSINo As String
    Property zrEndSINo As String
    Property zrBegTrxNo As String
    Property zrEndTrxNo As String
    Property zrCurrentTotalSales As Double
    Property zrBegBalance As Double
    Property zrEndBalance As Double
    Property zrAccumulatedGTS As Double
    Property zrResetCounter As Integer
    Property zrZCounter As Integer
    Property zrReprintCount As Integer
    '-------------------------SALES BY CLASS --------------------------------
    Property zrAddOns As Integer
    Property zrFamousBlends As Integer
    Property zrCombo As Integer
    Property zrPerfectC As Integer
    Property zrPremium As Integer
    Property zrSavory As Integer
    Property zrSimplyP As Integer
    '-------------------------CASH BREAKDOWN QTY--------------------------------
    Property zrThousandQty As Integer
    Property zrFiveHundredQty As Integer
    Property zrTwoHundredQty As Integer
    Property zrOneHundredQty As Integer
    Property zrFiftyQty As Integer
    Property zrTwentyQty As Integer
    Property zrTenQty As Integer
    Property zrFiveQty As Integer
    Property zrOneQty As Integer
    Property zrPointTwentyFiveQty As Integer
    Property zrPointFiveQty As Integer

    '-------------------------CASH BREAKDOWN TOTAL --------------------------------
    Property zrThousandTotal As Double
    Property zrFiveHundredTotal As Double
    Property zrTwoHundredTotal As Double
    Property zrOneHundredTotal As Double
    Property zrFiftyTotal As Double
    Property zrTwentyTotal As Double
    Property zrTenTotal As Double
    Property zrFiveTotal As Double
    Property zrOneTotal As Double
    Property zrPointTwentyFiveTotal As Double
    Property zrPointFiveTotal As Double
    Property zrCBQTYTotal As Integer
    Property zrCBGrandTotal As Double
    Property zrZReadDate As String
    Property zrDateFooter As String

    Public Sub FillReportValue(ZReadDate As String)
        Try
            'Total of Product Price(PHP 60.00)
            zrGrossSales = sum("grosssales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' AND active = 1")
            'Zero always
            zrLessVatDiplomat = 0
            'Gross Sales (xrGrossSales):60 / 1.12(Tax) = Vat Exempt (xrVatExempt):53.57 - Grosssales = 6.43(xrLessVatVE)
            zrLessVatVE = sum("lessvat", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Zero always
            zrLessVatOther = 0
            'Gross Sales (xrGrossSales):60 / 1.12 = Vatable Sales (xrVatableSales):53.57 - Grosssales = 6.43(xrVatAmount)
            zrVatAmount = sum("vatpercentage", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            ' Same value as Vat Amount (zrVatAmount)
            zrAddVat = zrVatAmount
            'Total Gift card value used
            zrGCUsed = sum("gc_value", "loc_coupon_data WHERE zreading = " & ZReadDate & " AND coupon_type = 'Fix-1'")
            'Total Value of Gift card
            zrGC = sum("coupon_total", "loc_coupon_data WHERE zreading = " & ZReadDate & " AND coupon_type = 'Fix-1' ")
            'Zero always
            zrGovTax = 0
            'Gross Sales (zrGrossSales):60 / 1.12(Tax) = Vatable Sales (zrVatableSales):53.57
            zrVatableSales = sum("vatablesales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")

            zrZeroRatedSales = sum("zeroratedsales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Gross Sales (xrGrossSales):60 / 1.12(Tax) = Vatable Exempt (xrVatExempt):53.57
            zrVatExempt = sum("vatexemptsales", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")
            'Total Discounts = Gross Sales ():60 / 1.12 = ():53.57 * 0.20 = 10.71(xrLessDiscVE) |OR| Used Gift Card(xrGCUsed)
            zrLessDiscVE = sum("coupon_total", $"loc_coupon_data WHERE zreading = '{ZReadDate}' AND coupon_type = 'Percentage(w/o vat)' AND status = 1")
            'Gross Sales (zrGrossSales) + Gift Card Used(zrGCSum) - Less Vat(zrLessVatVE) - Vat Amount()zrVatAmount - Discount (zrLessDiscVE)
            zrDailySales = zrGrossSales + zrGCUsed - zrLessVatVE - zrLessDiscVE
            'Gross Sales (zrGrossSales) + Gift Card Used(zrGCSum) - Less Vat(zrLessVatVE) - Vat Amount()zrVatAmount - Discount (zrLessDiscVE)


            zrCashTotal = sum("amountdue", $"loc_daily_transaction WHERE zreading = '{ZReadDate}'  AND active = 1")

            zrCreditCard = 0
            zrDebitCard = 0
            zrMisc = 0
            zrAR = 0
            zrTotalExpenses = sum("total_amount", $"loc_expense_list WHERE zreading = '{ZReadDate}' AND active = 1")
            zrOthers = 0
            zrCompBegBalance = returnselect("log_description", $"loc_system_logs WHERE log_type IN ('BG-1','BG-2','BG-3','BG-4') AND zreading = '{ZReadDate}' ORDER by log_date_time DESC LIMIT 1")
            zrDeposit = sum("amount", $"loc_deposit WHERE date(transaction_date) = '{ZReadDate}'")

            zrCashless = sum("amountdue", $"loc_daily_transaction WHERE active IN (1,3) AND zreading = '{ZReadDate}' AND transaction_type NOT IN ('Walk-in' , 'Complimentary Expenses')")
            zrGcash = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Gcash' ")
            zrPaymaya = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Paymaya' ")
            zrGrab = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Grab' ")
            zrFoodPanda = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Food Panda' ")
            zrShopee = sum("amountdue", $"loc_daily_transaction WHERE active = 1 AND zreading = '{ZReadDate}' AND transaction_type = 'Shopee' ")
            zrCompExpenses = sum("amountdue", $"loc_daily_transaction WHERE active = 3 AND zreading = '{ZReadDate}' AND transaction_type = 'Complimentary Expenses' ")
            zrCashlessOthers = sum("amountdue", $"loc_daily_transaction WHERE active = 3 AND zreading = '{ZReadDate}' AND transaction_type = 'Others' ")

            zrReturnEx = sum("quantity", $"loc_daily_transaction_details WHERE active = 2 AND zreading = '{ZReadDate}' ")
            zrReturnRef = sum("total", $"loc_daily_transaction_details WHERE active = 2 AND zreading = '{ZReadDate}' ")
            zrItemVoid = zrReturnEx
            zrTrxVoid = zrReturnEx
            zrTrxCancel = zrReturnEx
            zrDiplomat = 0

            zrTotalDisc = sum("coupon_total", $"loc_coupon_data WHERE zreading = '{ZReadDate}' AND status = 1")
            zrNetSales = zrGrossSales + zrGCUsed - zrLessVatVE - zrTotalDisc
            If zrCompBegBalance = "" Then zrCompBegBalance = 0
            zrCashinDrawer = zrGrossSales - zrCashless - zrTotalDisc - zrLessVatVE - zrTotalExpenses + Double.Parse(zrCompBegBalance)

            zrSeniorDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'Senior Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")
            zrPWDDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'PWD Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")
            zrAthleteDisc = sum("coupon_total", $"loc_coupon_data WHERE coupon_name = 'Sports Discount 20%' AND zreading = '{ZReadDate}' AND status = '1' ")
            zrSPDisc = 0
            zrDiscOthers = 0
            zrTakeOutCharge = 0
            zrDelCharge = 0

            zrTotalQTYSold = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND active = 1")
            zrTotalTrxCount = count("transaction_id", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ")
            zrTotalGuest = zrTotalTrxCount

            zrBegSINo = ReturnRowDouble("si_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' LIMIT 1")
            zrEndSINo = ReturnRowDouble("si_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ORDER by transaction_id DESC LIMIT 1")

            '-1 day in zread date
            Dim curDate As DateTime = DateTime.ParseExact(ZReadDate, "yyyy-MM-dd", Nothing)
            Dim formatCurDate As DateTime = curDate.AddDays(-1)
            Dim prevZreadDate = $"{formatCurDate.Year}-{formatCurDate.Month.ToString("D2")}-{formatCurDate.Day.ToString("D2")}"

            If zrBegSINo = 0 Then
                'Re query to get last si number from last zread date
                zrBegSINo = ReturnRowDouble("si_number", $"loc_daily_transaction ORDER by transaction_id DESC LIMIT 1")
                zrEndSINo = zrBegSINo
            End If

            Dim BegSi As Integer = Val(zrBegSINo)
            zrBegSINo = BegSi.ToString(S_SIFormat)

            Dim EndSI As Integer = Val(zrEndSINo)
            zrEndSINo = EndSI.ToString(S_SIFormat)

            zrBegTrxNo = returnselect("transaction_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' LIMIT 1")
            zrEndTrxNo = returnselect("transaction_number", $"loc_daily_transaction WHERE zreading = '{ZReadDate}' ORDER by transaction_id DESC LIMIT 1")

            If zrBegTrxNo = "" Then
                zrBegTrxNo = returnselect("transaction_number", $"loc_daily_transaction ORDER by transaction_id DESC LIMIT 1")
                zrEndTrxNo = zrBegTrxNo
            End If

            'Same value as Gross Sales
            zrCurrentTotalSales = zrGrossSales
            'On load value of Old grand total column in settings table
            zrBegBalance = S_OLDGRANDTOTAL

            zrAccumulatedGTS = S_OLDGRANDTOTAL + zrGrossSales
            zrEndBalance = zrAccumulatedGTS


            zrResetCounter = Val(returnselect("counter_value", "tbcountertable WHERE counter_id = 1"))
            zrZCounter = My.Settings.zcounter
            zrReprintCount = 0

            Dim CashBreakDownDatatable As DataTable = ReturnCashBreakdown(ZReadDate, ZReadDate)
            For Each row As DataRow In CashBreakDownDatatable.Rows
                zrThousandQty += row("Thousand")
                zrFiveHundredQty += row("FiveHundreds")
                zrTwoHundredQty += row("TwoHundred")
                zrOneHundredQty += row("OneHundred")
                zrFiftyQty += row("Fifty")
                zrTwentyQty += row("Twenty")
                zrTenQty += row("Ten")
                zrFiveQty += row("Five")
                zrOneQty += row("One")
                zrPointTwentyFiveQty += row("PointTwoFive")
                zrPointFiveQty += row("PointFive")
            Next row
            'With CashBreakDownDatatable
            '    For i As Integer = 0 To .Rows.Count - 1 Step +1
            '        zrThousandQty += CashBreakDownDatatable(i)(19)
            '        zrFiveHundredQty += CashBreakDownDatatable(i)(20)
            '        zrTwoHundredQty += CashBreakDownDatatable(i)(21)
            '        zrOneHundredQty += CashBreakDownDatatable(i)(22)
            '        zrFiftyQty += CashBreakDownDatatable(i)(23)
            '        zrTwentyQty += CashBreakDownDatatable(i)(24)
            '        zrTenQty += CashBreakDownDatatable(i)(25)
            '        zrFiveQty += CashBreakDownDatatable(i)(26)
            '        zrOneQty += CashBreakDownDatatable(i)(27)
            '        zrPointTwentyFiveQty += CashBreakDownDatatable(i)(28)
            '        zrPointFiveQty += CashBreakDownDatatable(i)(29)
            '    Next
            'End With

            zrThousandTotal = zrThousandQty * 1000
            zrFiveHundredTotal = zrFiveHundredQty * 500
            zrTwoHundredTotal = zrTwoHundredQty * 200
            zrOneHundredTotal = zrOneHundredQty * 100
            zrFiftyTotal = zrFiftyQty * 50
            zrTwentyTotal = zrTwentyQty * 20
            zrTenTotal = zrTenQty * 10
            zrFiveTotal = zrFiveQty * 5
            zrOneTotal = zrOneQty * 1
            zrPointTwentyFiveTotal = zrPointTwentyFiveQty * 0.25
            zrPointFiveTotal = zrPointFiveQty * 0.05

            zrAddOns = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Add-Ons'")
            zrFamousBlends = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Famous Blends'")
            zrCombo = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Combo'")
            zrPerfectC = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Perfect Combination'")
            zrPremium = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Premium'")
            zrSavory = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Savory'")
            zrSimplyP = sum("quantity", $"loc_daily_transaction_details WHERE zreading = '{ZReadDate}' AND product_category = 'Simply Perfect'")

            zrCBQTYTotal = zrThousandQty + zrFiveHundredQty + zrTwoHundredQty + zrOneHundredQty + zrFiftyQty + zrTenQty + zrFiveQty + zrOneQty + zrPointTwentyFiveQty + zrPointFiveQty
            zrCBGrandTotal = zrThousandTotal + zrFiveHundredTotal + zrTwoHundredTotal + zrOneHundredTotal + zrFiftyTotal + zrTwentyTotal + zrTenTotal + zrFiveTotal + zrOneTotal + zrPointTwentyFiveTotal + zrPointFiveTotal

            zrZReadDate = ZReadDate
            zrDateFooter = ZReadDate & " " & Format(Now(), "HH:mm:ss")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub zReadingBody(sender As Object, e As PrintPageEventArgs)
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
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TERMINAL NO.", zrTerminal, FontDefault, 5, 0)
            FillEJournalContent("TERMINAL NO.          " & zrTerminal, {"TERMINAL NO.", zrTerminal}, "LR", True, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GROSS", NUMBERFORMAT(zrGrossSales), FontDefault, 5, 0)
            FillEJournalContent("GROSS         " & NUMBERFORMAT(zrGrossSales), {"GROSS", NUMBERFORMAT(zrGrossSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (VE)", NUMBERFORMAT(zrLessVatVE), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (VE)         " & NUMBERFORMAT(zrLessVatVE), {"LESS VAT (VE)", NUMBERFORMAT(zrLessVatVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT DIPLOMAT", NUMBERFORMAT(zrLessVatDiplomat), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT DIPLOMAT         " & NUMBERFORMAT(zrLessVatDiplomat), {"LESS VAT DIPLOMAT", NUMBERFORMAT(zrLessVatDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (OTHER)", NUMBERFORMAT(zrLessVatOther), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (OTHER)          " & NUMBERFORMAT(zrLessVatOther), {"LESS VAT (OTHER)", NUMBERFORMAT(zrLessVatOther)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD VAT", NUMBERFORMAT(zrAddVat), FontDefault, 5, 0)
            FillEJournalContent("ADD VAT          " & NUMBERFORMAT(zrAddVat), {"ADD VAT", NUMBERFORMAT(zrAddVat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DAILY SALES", NUMBERFORMAT(zrDailySales), FontDefault, 5, 0)
            FillEJournalContent("DAILY SALES          " & NUMBERFORMAT(zrDailySales), {"DAILY SALES", NUMBERFORMAT(zrDailySales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT AMOUNT", NUMBERFORMAT(zrVatAmount), FontDefault, 5, 0)
            FillEJournalContent("VAT AMOUNT          " & NUMBERFORMAT(zrVatAmount), {"VAT AMOUNT", NUMBERFORMAT(zrVatAmount)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LOCAL GOV'T TAX", NUMBERFORMAT(zrGovTax), FontDefault, 5, 0)
            FillEJournalContent("LOCAL GOV'T TAX          " & NUMBERFORMAT(zrGovTax), {"LOCAL GOV'T TAX", NUMBERFORMAT(zrGovTax)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VATABLE SALES", NUMBERFORMAT(zrVatableSales), FontDefault, 5, 0)
            FillEJournalContent("VATABLE SALES          " & NUMBERFORMAT(zrVatableSales), {"VATABLE SALES", NUMBERFORMAT(zrVatableSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ZERO RATED SALES", NUMBERFORMAT(zrZeroRatedSales), FontDefault, 5, 0)
            FillEJournalContent("ZERO RATED SALES          " & NUMBERFORMAT(zrZeroRatedSales), {"ZERO RATED SALES", NUMBERFORMAT(zrZeroRatedSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT EXEMPT SALES", NUMBERFORMAT(zrVatExempt), FontDefault, 5, 0)
            FillEJournalContent("VAT EXEMPT SALES          " & NUMBERFORMAT(zrVatExempt), {"VAT EXEMPT SALES", NUMBERFORMAT(zrVatExempt)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS DISC (VE)", NUMBERFORMAT(zrLessDiscVE), FontDefault, 5, 0)
            FillEJournalContent("LESS DISC (VE)          " & NUMBERFORMAT(zrLessDiscVE), {"LESS DISC (VE)", NUMBERFORMAT(zrLessDiscVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "NET SALES", NUMBERFORMAT(zrNetSales), FontDefault, 5, 0)
            FillEJournalContent("NET SALES          " & NUMBERFORMAT(zrNetSales), {"NET SALES", NUMBERFORMAT(zrNetSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH TOTAL", NUMBERFORMAT(zrCashTotal), FontDefault, 5, 0)
            FillEJournalContent("CASH TOTAL          " & NUMBERFORMAT(zrCashTotal), {"CASH TOTAL", NUMBERFORMAT(zrCashTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CREDIT CARD", NUMBERFORMAT(zrCreditCard), FontDefault, 5, 0)
            FillEJournalContent("CREDIT CARD          " & NUMBERFORMAT(zrCreditCard), {"CREDIT CARD", NUMBERFORMAT(zrCreditCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEBIT CARD", NUMBERFORMAT(zrDebitCard), FontDefault, 5, 0)
            FillEJournalContent("DEBIT CARD          " & NUMBERFORMAT(zrDebitCard), {"DEBIT CARD", NUMBERFORMAT(zrDebitCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "MISC/CHEQUES", NUMBERFORMAT(zrMisc), FontDefault, 5, 0)
            FillEJournalContent("MISC/CHEQUES          " & NUMBERFORMAT(zrDebitCard), {"MISC/CHEQUES", NUMBERFORMAT(zrMisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD(GC)", NUMBERFORMAT(zrGC), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD(GC)          " & NUMBERFORMAT(zrGC), {"GIFT CARD(GC)", NUMBERFORMAT(zrGC)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD USED", NUMBERFORMAT(zrGCUsed), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD SUM          " & NUMBERFORMAT(zrGCUsed), {"GIFT CARD SUM", NUMBERFORMAT(zrGCUsed)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "A/R", NUMBERFORMAT(zrAR), FontDefault, 5, 0)
            FillEJournalContent("A/R          " & NUMBERFORMAT(zrAR), {"A/R", NUMBERFORMAT(zrAR)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL EXPENSES", NUMBERFORMAT(zrTotalExpenses), FontDefault, 5, 0)
            FillEJournalContent("TOTAL EXPENSES         " & NUMBERFORMAT(zrTotalExpenses), {"TOTAL EXPENSES", NUMBERFORMAT(zrTotalExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", "N/A", FontDefault, 5, 0)
            FillEJournalContent("OTHERS         N/A", {"OTHERS", "N/A"}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEG.BALANCE", NUMBERFORMAT(Double.Parse(zrCompBegBalance)), FontDefault, 5, 0)
            FillEJournalContent("BEG.BALANCE         " & NUMBERFORMAT(Double.Parse(zrCompBegBalance)), {"BEG.BALANCE", NUMBERFORMAT(Double.Parse(zrCompBegBalance))}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEPOSIT", NUMBERFORMAT(zrDeposit), FontDefault, 5, 0)
            FillEJournalContent("DEPOSIT          " & NUMBERFORMAT(zrDeposit), {"DEPOSIT", NUMBERFORMAT(zrDeposit)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH IN DRAWER", NUMBERFORMAT(zrCashinDrawer), FontDefault, 5, 0)
            FillEJournalContent("CASH IN DRAWER          " & NUMBERFORMAT(zrCashinDrawer), {"CASH IN DRAWER", NUMBERFORMAT(zrCashinDrawer)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASHLESS", NUMBERFORMAT(zrCashless), FontDefault, 5, 0)
            FillEJournalContent("CASHLESS          " & NUMBERFORMAT(zrCashless), {"CASHLESS", NUMBERFORMAT(zrCashless)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GCASH", NUMBERFORMAT(zrGcash), FontDefault, 5, 0)
            FillEJournalContent("GCASH          " & NUMBERFORMAT(zrGcash), {"GCASH", NUMBERFORMAT(zrGcash)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PAYMAYA", NUMBERFORMAT(zrPaymaya), FontDefault, 5, 0)
            FillEJournalContent("PAYMAYA          " & NUMBERFORMAT(zrPaymaya), {"PAYMAYA", NUMBERFORMAT(zrPaymaya)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SHOPEE", NUMBERFORMAT(zrShopee), FontDefault, 5, 0)
            FillEJournalContent("SHOPEE          " & NUMBERFORMAT(zrShopee), {"SHOPEE", NUMBERFORMAT(zrShopee)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FOODPANDA", NUMBERFORMAT(zrFoodPanda), FontDefault, 5, 0)
            FillEJournalContent("FOODPANDA          " & NUMBERFORMAT(zrFoodPanda), {"FOODPANDA", NUMBERFORMAT(zrFoodPanda)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAB", NUMBERFORMAT(zrGrab), FontDefault, 5, 0)
            FillEJournalContent("GRAB          " & NUMBERFORMAT(zrGrab), {"GRAB", NUMBERFORMAT(zrGrab)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMPLIMENTARY", NUMBERFORMAT(zrCompExpenses), FontDefault, 5, 0)
            FillEJournalContent("COMPLIMENTARY          " & NUMBERFORMAT(zrCompExpenses), {"COMPLIMENTARY", NUMBERFORMAT(zrCompExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", NUMBERFORMAT(zrCashlessOthers), FontDefault, 5, 0)
            FillEJournalContent("OTHERS          " & NUMBERFORMAT(zrCashlessOthers), {"OTHERS", NUMBERFORMAT(zrCashlessOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ITEM VOID E/C", NUMBERFORMAT(zrItemVoid), FontDefault, 5, 0)
            FillEJournalContent("ITEM VOID E/C          " & NUMBERFORMAT(zrItemVoid), {"ITEM VOID E/C", NUMBERFORMAT(zrItemVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION VOID", NUMBERFORMAT(zrTrxVoid), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION VOID         " & NUMBERFORMAT(zrTrxVoid), {"TRANSACTION VOID", NUMBERFORMAT(zrTrxVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION CANCEL", NUMBERFORMAT(zrTrxCancel), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION CANCEL         " & NUMBERFORMAT(zrTrxCancel), {"TRANSACTION CANCEL", NUMBERFORMAT(zrTrxCancel)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DIMPLOMAT", NUMBERFORMAT(zrDiplomat), FontDefault, 5, 0)
            FillEJournalContent("DIMPLOMAT         " & NUMBERFORMAT(zrDiplomat), {"DIMPLOMAT", NUMBERFORMAT(zrDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL DISCOUNTS", NUMBERFORMAT(zrTotalDisc), FontDefault, 5, 0)
            FillEJournalContent("TOTAL DISCOUNTS         " & NUMBERFORMAT(zrTotalDisc), {"TOTAL DISCOUNTS", NUMBERFORMAT(zrTotalDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - SENIOR CITIZEN", NUMBERFORMAT(zrSeniorDisc), FontDefault, 5, 0)
            FillEJournalContent(" - SENIOR CITIZEN         " & NUMBERFORMAT(zrSeniorDisc), {" - SENIOR CITIZEN", NUMBERFORMAT(zrSeniorDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - PWD", NUMBERFORMAT(zrPWDDisc), FontDefault, 5, 0)
            FillEJournalContent(" - PWD         " & NUMBERFORMAT(zrPWDDisc), {" -PWD", NUMBERFORMAT(zrPWDDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - ATHLETE", NUMBERFORMAT(zrAthleteDisc), FontDefault, 5, 0)
            FillEJournalContent(" - ATHLETE         " & NUMBERFORMAT(zrAthleteDisc), {" - ATHLETE", NUMBERFORMAT(zrAthleteDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - OTHERS", NUMBERFORMAT(zrDiscOthers), FontDefault, 5, 0)
            FillEJournalContent(" - OTHERS         " & NUMBERFORMAT(zrDiscOthers), {" - OTHERS", NUMBERFORMAT(zrDiscOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TAKE OUT CHARGE", NUMBERFORMAT(zrTakeOutCharge), FontDefault, 5, 0)
            FillEJournalContent("TAKE OUT CHARGE         " & NUMBERFORMAT(zrTakeOutCharge), {"TAKE OUT CHARGE", NUMBERFORMAT(zrTakeOutCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DELIVERY CHARGE", NUMBERFORMAT(zrDelCharge), FontDefault, 5, 0)
            FillEJournalContent("DELIVERY CHARGE         " & NUMBERFORMAT(zrDelCharge), {"DELIVERY CHARGE", NUMBERFORMAT(zrDelCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS EXCHANGE", NUMBERFORMAT(zrReturnEx), FontDefault, 5, 0)
            FillEJournalContent("RETURNS EXCHANGE         " & NUMBERFORMAT(zrReturnEx), {"RETURNS EXCHANGE", NUMBERFORMAT(zrReturnEx)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS REFUND", NUMBERFORMAT(zrReturnRef), FontDefault, 5, 0)
            FillEJournalContent("RETURNS REFUND         " & NUMBERFORMAT(zrReturnRef), {"RETURNS REFUND", NUMBERFORMAT(zrReturnRef)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL QTY SOLD", zrTotalQTYSold, FontDefault, 5, 0)
            FillEJournalContent("TOTAL QTY SOLD         " & zrTotalQTYSold, {"TOTAL QTY SOLD", zrTotalQTYSold}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL TRANS. COUNT", zrTotalTrxCount, FontDefault, 5, 0)
            FillEJournalContent("TOTAL TRANS. COUNT         " & zrTotalTrxCount, {"TOTAL TRANS. COUNT", zrTotalTrxCount}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL GUEST", zrTotalGuest, FontDefault, 5, 0)
            FillEJournalContent("TOTAL GUEST         " & zrTotalGuest, {"TOTAL GUEST", zrTotalGuest}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING S.I NO.", zrBegSINo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING S.I NO.         " & zrBegSINo, {"BEGINNING S.I NO.", zrBegSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END S.I NO.", zrEndSINo, FontDefault, 5, 0)
            FillEJournalContent("END S.I NO.         " & zrEndSINo, {"END S.I NO.", zrEndSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING TRANS. NO.", zrBegTrxNo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING TRANS. NO.         " & zrBegTrxNo, {"BEGINNING TRANS. NO.", zrBegTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END TRANS. NO.", zrEndTrxNo, FontDefault, 5, 0)
            FillEJournalContent("END TRANS. NO.         " & zrEndTrxNo, {"END TRANS. NO.", zrEndTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CURRENT TOTAL SALES", NUMBERFORMAT(zrCurrentTotalSales), FontDefault, 5, 0)
            FillEJournalContent("CURRENT TOTAL SALES         " & NUMBERFORMAT(zrCurrentTotalSales), {"CURRENT TOTAL SALES", NUMBERFORMAT(zrCurrentTotalSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING BALANCE", NUMBERFORMAT(zrBegBalance), FontDefault, 5, 0)
            FillEJournalContent("BEGINNING BALANCE         " & NUMBERFORMAT(zrBegBalance), {"BEGINNING BALANCE", NUMBERFORMAT(zrBegBalance)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ENDING BALANCE", NUMBERFORMAT(zrEndBalance), FontDefault, 5, 0)
            FillEJournalContent("ENDING BALANCE         " & NUMBERFORMAT(zrEndBalance), {"ENDING BALANCE", NUMBERFORMAT(zrEndBalance)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ACCUMULATED GRAND TOTAL SALES", NUMBERFORMAT(zrAccumulatedGTS), FontDefault, 5, 0)
            FillEJournalContent("ACCUMULATED GRAND TOTAL SALES         " & NUMBERFORMAT(zrAccumulatedGTS), {"ACCUMULATED GRAND TOTAL SALES", NUMBERFORMAT(zrAccumulatedGTS)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RESET COUNTER", NUMBERFORMAT(zrResetCounter), FontDefault, 5, 0)
            FillEJournalContent("RESET COUNTER         " & NUMBERFORMAT(zrResetCounter), {"RESET COUNTER", NUMBERFORMAT(zrResetCounter)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "Z-COUNTER", NUMBERFORMAT(zrZCounter), FontDefault, 5, 0)
            FillEJournalContent("Z-COUNTER         " & NUMBERFORMAT(zrZCounter), {"Z-COUNTER", NUMBERFORMAT(zrZCounter)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RE-PRINT COUNT", NUMBERFORMAT(zrReprintCount), FontDefault, 5, 0)
            FillEJournalContent("RE-PRINT COUNT         " & NUMBERFORMAT(zrReprintCount), {"RE-PRINT COUNT", NUMBERFORMAT(zrReprintCount)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "SALES BY CLASS", FontDefault, 0, RECEIPTLINECOUNT)
            FillEJournalContent("SALES BY CLASS", {}, "S", False, True)
            RECEIPTLINECOUNT += 30
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD ONS", zrAddOns, FontDefault, 5, 0)
            FillEJournalContent("ADD ONS          " & zrAddOns, {"ADD ONS", zrAddOns}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FAMOUS BLENDS", zrFamousBlends, FontDefault, 5, 0)
            FillEJournalContent("FAMOUS BLENDS          " & zrFamousBlends, {"FAMOUS BLENDS", zrFamousBlends}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMBO", zrCombo, FontDefault, 5, 0)
            FillEJournalContent("COMBO          " & zrCombo, {"COMBO", zrCombo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PERFECT COMBINATION", zrPerfectC, FontDefault, 5, 0)
            FillEJournalContent("PERFECT COMBINATION          " & zrPerfectC, {"PERFECT COMBINATION", zrPerfectC}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PREMIUM LINE", zrPremium, FontDefault, 5, 0)
            FillEJournalContent("PREMIUM LINE          " & zrPremium, {"PREMIUM LINE", zrPremium}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SAVORY", zrSavory, FontDefault, 5, 0)
            FillEJournalContent("SAVORY          " & zrSavory, {"SAVORY", zrSavory}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SIMPY PERFECT", zrSimplyP, FontDefault, 5, 0)
            FillEJournalContent("SIMPY PERFECT          " & zrSimplyP, {"SIMPY PERFECT", zrSimplyP}, "LR", False, False)
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
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1000", NUMBERFORMAT(zrThousandTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrThousandQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1000           " & zrThousandQty & "           " & NUMBERFORMAT(zrThousandTotal), {"₱ 1000", zrThousandQty, NUMBERFORMAT(zrThousandTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 500", NUMBERFORMAT(zrFiveHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrFiveHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 500           " & zrFiveHundredQty & "           " & NUMBERFORMAT(zrFiveHundredTotal), {"₱ 500", zrFiveHundredQty, NUMBERFORMAT(zrFiveHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 200", NUMBERFORMAT(zrTwoHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrTwoHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 200           " & zrTwoHundredQty & "           " & NUMBERFORMAT(zrTwoHundredTotal), {"₱ 200", zrTwoHundredQty, NUMBERFORMAT(zrTwoHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 100", NUMBERFORMAT(zrOneHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrOneHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 100           " & zrOneHundredQty & "           " & NUMBERFORMAT(zrOneHundredTotal), {"₱ 100", zrOneHundredQty, NUMBERFORMAT(zrOneHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 50", NUMBERFORMAT(zrFiftyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrFiftyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 50           " & zrFiftyQty & "           " & NUMBERFORMAT(zrFiftyTotal), {"₱ 50", zrFiftyQty, NUMBERFORMAT(zrFiftyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 20", NUMBERFORMAT(zrTwentyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrTwentyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 20           " & zrTwentyQty & "           " & NUMBERFORMAT(zrTwentyTotal), {"₱ 20", zrTwentyQty, NUMBERFORMAT(zrTwentyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 10", NUMBERFORMAT(zrTenTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrTenQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 10           " & zrTenQty & "           " & NUMBERFORMAT(zrTenTotal), {"₱ 10", zrTenQty, NUMBERFORMAT(zrTenTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 5", NUMBERFORMAT(zrFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 5           " & zrFiveQty & "           " & NUMBERFORMAT(zrFiveTotal), {"₱ 5", zrFiveQty, NUMBERFORMAT(zrFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1", NUMBERFORMAT(zrOneTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrOneQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1           " & zrOneQty & "           " & NUMBERFORMAT(zrOneTotal), {"₱ 1", zrOneQty, NUMBERFORMAT(zrOneTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .25", NUMBERFORMAT(zrPointTwentyFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrPointTwentyFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .25           " & zrPointTwentyFiveQty & "           " & NUMBERFORMAT(zrPointTwentyFiveTotal), {"₱ .25", zrPointTwentyFiveQty, NUMBERFORMAT(zrPointTwentyFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .05", NUMBERFORMAT(zrPointFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrPointFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .05           " & zrPointFiveQty & "           " & NUMBERFORMAT(zrPointFiveTotal), {"₱ .05", zrPointFiveQty, NUMBERFORMAT(zrPointFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT -= 10
            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 30

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAND TOTAL", NUMBERFORMAT(zrCBGrandTotal), FontDefaultBold, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrCBQTYTotal, FontDefaultBold, 0, 110)
            FillEJournalContent("GRAND TOTAL          " & NUMBERFORMAT(zrCBGrandTotal), {"GRAND TOTAL", NUMBERFORMAT(zrCBGrandTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            CenterTextDisplay(sender, e, zrDateFooter, FontDefault, RECEIPTLINECOUNT)
            FillEJournalContent(zrDateFooter, {}, "C", False, True)

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Property ZReadTable = "loc_zread_table"
    Property ZReadTableFields = "`ZXTerminal`, `ZXGross`, `ZXLessVat`, `ZXLessVatDiplomat`, `ZXLessVatOthers`, `ZXAddVat`, `ZXDailySales`, `ZXVatAmount`, `ZXLocalGovTax`, `ZXVatableSales`, `ZXZeroRatedSales`, `ZXVatExemptSales`, `ZXLessDiscVE`, `ZXNetSales`, `ZXCashTotal`, `ZXCreditCard`, `ZXDebitCard`, `ZXMiscCheques`, `ZXGiftCard`, `ZXGiftCardSum`, `ZXAR`, `ZXTotalExpenses`, `ZXCardOthers`, `ZXCompBegBal`, `ZXDeposits`, `ZXCashInDrawer`, `ZXCashlessTotal`, `ZXGcash`, `ZXPaymaya`, `ZXShopeePay`, `ZXFoodPanda`, `ZXGrabFood`, `ZXRepExpense`, `ZXCashlessOthers`, `ZXItemVoidEC`, `ZXTransactionVoid`, `ZXTransactionCancel`, `ZXDiplomat`, `ZXTotalDiscounts`, `ZXSeniorCitizen`, `ZXPWD`, `ZXAthlete`, `ZXSingleParent`, `ZXDiscOthers`, `ZXTakeOutCharge`, `ZXDeliveryCharge`, `ZXReturnsExchange`, `ZXReturnsRefund`, `ZXTotalQTYSold`, `ZXTotalTransactionCount`, `ZXTotalGuess`, `ZXBegSiNo`, `ZXEndSINo`, `ZXBegTransNo`, `ZXEndTransNo`, `ZXCurrentTotalSales`, `ZXBegBalance`, `ZXEndingBalance`, `ZXAccumulatedGT`, `ZXResetCounter`, `ZXZCounter`, `ZXReprintCount`, `ZXAddOns`, `ZXFamousBlends`, `ZXCombo`, `ZXPerfectCombination`, `ZXPremium`, `ZXSavoury`, `ZXSimplyPerfect`, `ZXThousandQty`, `ZXFiveHundredQty`, `ZXTwoHundredQty`, `ZXOneHundredQty`, `ZXFiftyQty`, `ZXTwentyQty`, `ZXTenQty`, `ZXFiveQty`, `ZXOneQty`, `ZXPointTwentyFiveQty`, `ZXPointFiveQty`, `ZXThousandTotal`, `ZXFiveHundredTotal`, `ZXTwoHundredTotal`, `ZXOneHundredTotal`, `ZXFiftyTotal`, `ZXTwentyTotal`, `ZXTenTotal`, `ZXFiveTotal`, `ZXOneTotal`, `ZXPointTwentyFiveTotal`, `ZXPointFiveTotal`, `ZXdate`, `ZXCBQtyTotal`, `ZXCBGrandTotal`, `ZXDateFooter`, `created_by`"
    Property ZReadValues = "@ZXTerminal, @ZXGross, @ZXLessVat, @ZXLessVatDiplomat, @ZXLessVatOthers, @ZXAddVat, @ZXDailySales, @ZXVatAmount, @ZXLocalGovTax, @ZXVatableSales, @ZXZeroRatedSales, @ZXVatExemptSales, @ZXLessDiscVE, @ZXNetSales, @ZXCashTotal, @ZXCreditCard, @ZXDebitCard, @ZXMiscCheques, @ZXGiftCard, @ZXGiftCardSum, @ZXAR, @ZXTotalExpenses, @ZXCardOthers, @ZXCompBegBal, @ZXDeposits, @ZXCashInDrawer, @ZXCashlessTotal, @ZXGcash, @ZXPaymaya, @ZXShopeePay, @ZXFoodPanda, @ZXGrabFood, @ZXRepExpense, @ZXCashlessOthers, @ZXItemVoidEC, @ZXTransactionVoid, @ZXTransactionCancel, @ZXDiplomat, @ZXTotalDiscounts, @ZXSeniorCitizen, @ZXPWD, @ZXAthlete, @ZXSingleParent, @ZXDiscOthers, @ZXTakeOutCharge, @ZXDeliveryCharge, @ZXReturnsExchange, @ZXReturnsRefund, @ZXTotalQTYSold, @ZXTotalTransactionCount, @ZXTotalGuess, @ZXBegSiNo, @ZXEndSINo, @ZXBegTransNo, @ZXEndTransNo, @ZXCurrentTotalSales, @ZXBegBalance, @ZXEndingBalance, @ZXAccumulatedGT, @ZXResetCounter, @ZXZCounter, @ZXReprintCount, @ZXAddOns, @ZXFamousBlends, @ZXCombo, @ZXPerfectCombination, @ZXPremium, @ZXSavoury, @ZXSimplyPerfect, @ZXThousandQty, @ZXFiveHundredQty, @ZXTwoHundredQty, @ZXOneHundredQty, @ZXFiftyQty, @ZXTwentyQty, @ZXTenQty, @ZXFiveQty, @ZXOneQty, @ZXPointTwentyFiveQty, @ZXPointFiveQty, @ZXThousandTotal, @ZXFiveHundredTotal, @ZXTwoHundredTotal, @ZXOneHundredTotal, @ZXFiftyTotal, @ZXTwentyTotal, @ZXTenTotal, @ZXFiveTotal, @ZXOneTotal, @ZXPointTwentyFiveTotal, @ZXPointFiveTotal, @ZXdate, @ZXCBQtyTotal, @ZXCBGrandTotal, @ZXDateFooter, @created_by"

    Public Sub InsertZReadXRead()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "INSERT INTO " & ZReadTable & " (" & ZReadTableFields & ") VALUES (" & ZReadValues & ")"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)

            Command.Parameters.AddWithValue("@ZXTerminal", zrTerminal)
            Command.Parameters.AddWithValue("@ZXGross", zrGrossSales)
            Command.Parameters.AddWithValue("@ZXLessVat", zrLessVatVE)
            Command.Parameters.AddWithValue("@ZXLessVatDiplomat", zrLessVatDiplomat)
            Command.Parameters.AddWithValue("@ZXLessVatOthers", zrLessVatOther)
            Command.Parameters.AddWithValue("@ZXAddVat", zrAddVat)
            Command.Parameters.AddWithValue("@ZXDailySales", zrDailySales)
            Command.Parameters.AddWithValue("@ZXVatAmount", zrVatAmount)
            Command.Parameters.AddWithValue("@ZXLocalGovTax", zrGovTax)
            Command.Parameters.AddWithValue("@ZXVatableSales", zrVatableSales)
            Command.Parameters.AddWithValue("@ZXZeroRatedSales", zrZeroRatedSales)
            Command.Parameters.AddWithValue("@ZXVatExemptSales", zrVatExempt)
            Command.Parameters.AddWithValue("@ZXLessDiscVE", zrLessDiscVE)
            Command.Parameters.AddWithValue("@ZXNetSales", zrNetSales)
            Command.Parameters.AddWithValue("@ZXCashTotal", zrCashTotal)
            Command.Parameters.AddWithValue("@ZXCreditCard", zrCreditCard)
            Command.Parameters.AddWithValue("@ZXDebitCard", zrDebitCard)
            Command.Parameters.AddWithValue("@ZXMiscCheques", zrMisc)
            Command.Parameters.AddWithValue("@ZXGiftCard", zrGC)
            Command.Parameters.AddWithValue("@ZXGiftCardSum", zrGCUsed)
            Command.Parameters.AddWithValue("@ZXAR", zrAR)
            Command.Parameters.AddWithValue("@ZXTotalExpenses", zrTotalExpenses)
            Command.Parameters.AddWithValue("@ZXCardOthers", zrOthers)
            Command.Parameters.AddWithValue("@ZXCompBegBal", Double.Parse(zrCompBegBalance))
            Command.Parameters.AddWithValue("@ZXDeposits", zrDeposit)
            Command.Parameters.AddWithValue("@ZXCashInDrawer", zrCashinDrawer)
            Command.Parameters.AddWithValue("@ZXCashlessTotal", zrCashless)
            Command.Parameters.AddWithValue("@ZXGcash", zrGcash)
            Command.Parameters.AddWithValue("@ZXPaymaya", zrPaymaya)
            Command.Parameters.AddWithValue("@ZXShopeePay", zrShopee)
            Command.Parameters.AddWithValue("@ZXFoodPanda", zrFoodPanda)
            Command.Parameters.AddWithValue("@ZXGrabFood", zrGrab)
            Command.Parameters.AddWithValue("@ZXRepExpense", zrCompExpenses)
            Command.Parameters.AddWithValue("@ZXCashlessOthers", zrCashlessOthers)
            Command.Parameters.AddWithValue("@ZXItemVoidEC", zrItemVoid)
            Command.Parameters.AddWithValue("@ZXTransactionVoid", zrTrxVoid)
            Command.Parameters.AddWithValue("@ZXTransactionCancel", zrTrxCancel)
            Command.Parameters.AddWithValue("@ZXDiplomat", zrDiplomat)
            Command.Parameters.AddWithValue("@ZXTotalDiscounts", zrTotalDisc)
            Command.Parameters.AddWithValue("@ZXSeniorCitizen", zrSeniorDisc)
            Command.Parameters.AddWithValue("@ZXPWD", zrPWDDisc)
            Command.Parameters.AddWithValue("@ZXAthlete", zrAthleteDisc)
            Command.Parameters.AddWithValue("@ZXSingleParent", zrSPDisc)
            Command.Parameters.AddWithValue("@ZXDiscOthers", zrDiscOthers)
            Command.Parameters.AddWithValue("@ZXTakeOutCharge", zrTakeOutCharge)
            Command.Parameters.AddWithValue("@ZXDeliveryCharge", zrDelCharge)
            Command.Parameters.AddWithValue("@ZXReturnsExchange", zrReturnEx)
            Command.Parameters.AddWithValue("@ZXReturnsRefund", zrReturnRef)
            Command.Parameters.AddWithValue("@ZXTotalQTYSold", zrTotalQTYSold)
            Command.Parameters.AddWithValue("@ZXTotalTransactionCount", zrTotalTrxCount)
            Command.Parameters.AddWithValue("@ZXTotalGuess", zrTotalGuest)
            Command.Parameters.AddWithValue("@ZXBegSiNo", zrBegSINo)
            Command.Parameters.AddWithValue("@ZXEndSINo", zrEndSINo)
            Command.Parameters.AddWithValue("@ZXBegTransNo", zrBegTrxNo)
            Command.Parameters.AddWithValue("@ZXEndTransNo", zrEndTrxNo)
            Command.Parameters.AddWithValue("@ZXCurrentTotalSales", zrCurrentTotalSales)
            Command.Parameters.AddWithValue("@ZXBegBalance", zrBegBalance)
            Command.Parameters.AddWithValue("@ZXEndingBalance", zrEndBalance)
            Command.Parameters.AddWithValue("@ZXAccumulatedGT", zrAccumulatedGTS)
            Command.Parameters.AddWithValue("@ZXResetCounter", zrResetCounter)
            Command.Parameters.AddWithValue("@ZXZCounter", zrZCounter)
            Command.Parameters.AddWithValue("@ZXReprintCount", zrReprintCount)
            Command.Parameters.AddWithValue("@ZXAddOns", zrAddOns)
            Command.Parameters.AddWithValue("@ZXFamousBlends", zrFamousBlends)
            Command.Parameters.AddWithValue("@ZXCombo", zrCombo)
            Command.Parameters.AddWithValue("@ZXPerfectCombination", zrPerfectC)
            Command.Parameters.AddWithValue("@ZXPremium", zrPremium)
            Command.Parameters.AddWithValue("@ZXSavoury", zrSavory)
            Command.Parameters.AddWithValue("@ZXSimplyPerfect", zrSimplyP)
            Command.Parameters.AddWithValue("@ZXThousandQty", zrThousandQty)
            Command.Parameters.AddWithValue("@ZXFiveHundredQty", zrFiveHundredQty)
            Command.Parameters.AddWithValue("@ZXTwoHundredQty", zrTwoHundredQty)
            Command.Parameters.AddWithValue("@ZXOneHundredQty", zrOneHundredQty)
            Command.Parameters.AddWithValue("@ZXFiftyQty", zrFiftyQty)
            Command.Parameters.AddWithValue("@ZXTwentyQty", zrTwentyQty)
            Command.Parameters.AddWithValue("@ZXTenQty", zrTenQty)
            Command.Parameters.AddWithValue("@ZXFiveQty", zrFiveQty)
            Command.Parameters.AddWithValue("@ZXOneQty", zrOneQty)
            Command.Parameters.AddWithValue("@ZXPointTwentyFiveQty", zrPointTwentyFiveQty)
            Command.Parameters.AddWithValue("@ZXPointFiveQty", zrPointFiveQty)
            Command.Parameters.AddWithValue("@ZXThousandTotal", zrThousandTotal)
            Command.Parameters.AddWithValue("@ZXFiveHundredTotal", zrFiveHundredTotal)
            Command.Parameters.AddWithValue("@ZXTwoHundredTotal", zrTwoHundredTotal)
            Command.Parameters.AddWithValue("@ZXOneHundredTotal", zrOneHundredTotal)
            Command.Parameters.AddWithValue("@ZXFiftyTotal", zrFiftyTotal)
            Command.Parameters.AddWithValue("@ZXTwentyTotal", zrTwentyTotal)
            Command.Parameters.AddWithValue("@ZXTenTotal", zrTenTotal)
            Command.Parameters.AddWithValue("@ZXFiveTotal", zrFiveTotal)
            Command.Parameters.AddWithValue("@ZXOneTotal", zrOneTotal)
            Command.Parameters.AddWithValue("@ZXPointTwentyFiveTotal", zrPointTwentyFiveTotal)
            Command.Parameters.AddWithValue("@ZXPointFiveTotal", zrPointFiveTotal)
            Command.Parameters.AddWithValue("@ZXdate", zrZReadDate)
            Command.Parameters.AddWithValue("@ZXCBQtyTotal", zrCBQTYTotal)
            Command.Parameters.AddWithValue("@ZXCBGrandTotal", zrCBGrandTotal)
            Command.Parameters.AddWithValue("@ZXDateFooter", zrDateFooter)
            Command.Parameters.AddWithValue("@created_by", ClientCrewID)
            Command.ExecuteNonQuery()

        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ModZXRead/InsertZReadXRead(): " & ex.ToString, "Critical")
        End Try
    End Sub

    Public Function UpdateBegBalance() As Double
        Try

            zrBegBalance += zrGrossSales
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query = "UPDATE loc_settings SET S_Old_Grand_Total = " & zrBegBalance & " WHERE settings_id = 1"
            Dim Cmd As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            Cmd.ExecuteNonQuery()
            ConnectionLocal.Close()
            Cmd.Dispose()
        Catch ex As Exception
            AuditTrail.LogToAuditTrail("System", "ModRet/GetOldGrandtotal(): " & ex.ToString, "Critical")
        End Try
        Return zrBegBalance
    End Function
    Public Function UpdateZreadSettings()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            zrZReadDate = Format(DateAdd("d", 1, zrZReadDate), "yyyy-MM-dd")
            sql = "UPDATE loc_settings SET S_Zreading = '" & zrZReadDate & "'"
            cmd = New MySqlCommand(sql, ConnectionLocal)
            cmd.ExecuteNonQuery()
            cmd.Dispose()

            sql = "UPDATE loc_pos_inventory SET zreading = '" & zrZReadDate & "'"
            cmd = New MySqlCommand(sql, ConnectionLocal)
            cmd.ExecuteNonQuery()

            cmd.Dispose()
            ConnectionLocal.Close()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return zrZReadDate
    End Function
End Class
