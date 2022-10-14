Imports System.Drawing.Printing
Imports MySql.Data.MySqlClient

Public Class zreadReprintCls
    Property zrppConnStr As String
    Property Title As String = "ZREAD"
    Property zrpTerminal As String = S_Terminal_No
    Property zrpGrossSales As Double
    Property zrpLessVatVE As Double
    Property zrpLessVatDiplomat As Double
    Property zrpLessVatOther As Double
    Property zrpAddVat As Double
    Property zrpDailySales As Double
    '--------------------------------------------------------
    Property zrpVatAmount As Double
    Property zrpGovTax As Double
    Property zrpVatableSales As Double
    Property zrpZeroRatedSales As Double
    Property zrpVatExempt As Double 'Vat Exempt Sales
    Property zrpLessDiscVE As Double 'Coupon Total
    Property zrpNetSales As Double 'Amount Due
    '-------------------------CASH--------------------------------
    Property zrpCashTotal As Double
    Property zrpCreditCard As Double
    Property zrpDebitCard As Double
    Property zrpMisc As Double
    Property zrpGC As Double
    Property zrpGCUsed As Double
    Property zrpAR As Double
    Property zrpTotalExpenses As Double
    Property zrpOthers As Double 'N/A
    Property zrpCompBegBalance As Double
    Property zrpDeposit As Double
    Property zrpCashinDrawer As Double
    '-------------------------CASH LESS--------------------------------
    Property zrpCashless As Double
    Property zrpGcash As Double
    Property zrpPaymaya As Double
    Property zrpShopee As Double
    Property zrpFoodPanda As Double
    Property zrpGrab As Double
    Property zrpCompExpenses As Double
    Property zrpCashlessOthers As Double
    '---------------------------------------------------------
    Property zrpItemVoid As Double
    Property zrpTrxVoid As Double
    Property zrpTrxCancel As Double
    Property zrpDiplomat As Double
    Property zrpTotalDisc As Double
    Property zrpSeniorDisc As Double
    Property zrpPWDDisc As Double
    Property zrpAthleteDisc As Double
    Property zrpSPDisc As Double
    Property zrpDiscOthers As Double
    Property zrpTakeOutCharge As Double
    Property zrpDelCharge As Double
    Property zrpReturnEx As Double
    Property zrpReturnRef As Double
    Property zrpTotalQTYSold As Integer
    Property zrpTotalTrxCount As Integer
    Property zrpTotalGuest As Integer
    Property zrpBegSINo As String
    Property zrpEndSINo As String
    Property zrpBegTrxNo As String
    Property zrpEndTrxNo As String
    Property zrpCurrentTotalSales As Double
    Property zrpBegBalance As Double
    Property zrpEndBalance As Double
    Property zrpAccumulatedGTS As Double
    Property zrpResetCounter As Integer
    Property zrpZCounter As Integer
    Property zrpReprintCount As Integer
    '-------------------------SALES BY CLASS --------------------------------
    Property zrpAddOns As Integer
    Property zrpFamousBlends As Integer
    Property zrpCombo As Integer
    Property zrpPerfectC As Integer
    Property zrpPremium As Integer
    Property zrpSavory As Integer
    Property zrpSimplyP As Integer
    '-------------------------CASH BREAKDOWN QTY--------------------------------
    Property zrpThousandQty As Integer
    Property zrpFiveHundredQty As Integer
    Property zrpTwoHundredQty As Integer
    Property zrpOneHundredQty As Integer
    Property zrpFiftyQty As Integer
    Property zrpTwentyQty As Integer
    Property zrpTenQty As Integer
    Property zrpFiveQty As Integer
    Property zrpOneQty As Integer
    Property zrpPointTwentyFiveQty As Integer
    Property zrpPointFiveQty As Integer

    '-------------------------CASH BREAKDOWN TOTAL --------------------------------
    Property zrpThousandTotal As Double
    Property zrpFiveHundredTotal As Double
    Property zrpTwoHundredTotal As Double
    Property zrpOneHundredTotal As Double
    Property zrpFiftyTotal As Double
    Property zrpTwentyTotal As Double
    Property zrpTenTotal As Double
    Property zrpFiveTotal As Double
    Property zrpOneTotal As Double
    Property zrpPointTwentyFiveTotal As Double
    Property zrpPointFiveTotal As Double
    Property zrpCBQTYTotal As Integer
    Property zrpCBGrandTotal As Double
    Property zrpDateFooterFrom As String
    Property zrpDateFooterTo As String
    Property zrpZreadTable = "ZXTerminal, SUM(ZXGross) as ZXGross, SUM(ZXLessVat) as ZXLessVat, SUM(ZXLessVatDiplomat) as ZXLessVatDiplomat, SUM(ZXLessVatOthers) as ZXLessVatOthers, SUM(ZXAddVat) as ZXAddVat, 
    SUM(ZXDailySales) as ZXDailySales, SUM(ZXVatAmount) as ZXVatAmount, SUM(ZXLocalGovTax) as ZXLocalGovTax, SUM(ZXVatableSales) as ZXVatableSales, 
    SUM(ZXZeroRatedSales) as ZXZeroRatedSales, SUM(ZXVatExemptSales) as ZXVatExemptSales, SUM(ZXLessDiscVE) as ZXLessDiscVE, SUM(ZXNetSales) as ZXNetSales, 
    SUM(ZXCashTotal) as ZXCashTotal, SUM(ZXCreditCard) as ZXCreditCard, SUM(ZXDebitCard) as ZXDebitCard, SUM(ZXMiscCheques) as ZXMiscCheques, 
    SUM(ZXGiftCard) as ZXGiftCard, SUM(ZXGiftCardSum) as ZXGiftCardSum, SUM(ZXAR) as ZXAR, SUM(ZXTotalExpenses) as ZXTotalExpenses, 
    SUM(ZXCardOthers) as ZXCardOthers, SUM(ZXCompBegBal) as ZXCompBegBal, SUM(ZXDeposits) as ZXDeposits, SUM(ZXCashInDrawer) as ZXCashInDrawer, 
    SUM(ZXCashlessTotal) as ZXCashlessTotal, SUM(ZXGcash) as ZXGcash, SUM(ZXPaymaya) as ZXPaymaya, SUM(ZXShopeePay) as ZXShopeePay, 
    SUM(ZXFoodPanda) as ZXFoodPanda, SUM(ZXGrabFood) as ZXGrabFood, SUM(ZXRepExpense) as ZXRepExpense, SUM(ZXCashlessOthers) as ZXCashlessOthers, 
    SUM(ZXItemVoidEC) as ZXItemVoidEC, SUM(ZXTransactionVoid) as ZXTransactionVoid, SUM(ZXTransactionCancel) as ZXTransactionCancel, 
    SUM(ZXDiplomat) as ZXDiplomat, SUM(ZXTotalDiscounts) as ZXTotalDiscounts, SUM(ZXSeniorCitizen) as ZXSeniorCitizen, SUM(ZXPWD) as ZXPWD, 
    SUM(ZXAthlete) as ZXAthlete, SUM(ZXSingleParent) as ZXSingleParent, SUM(ZXDiscOthers) as ZXDiscOthers, SUM(ZXTakeOutCharge) as ZXTakeOutCharge, 
    SUM(ZXDeliveryCharge) as ZXDeliveryCharge, SUM(ZXReturnsExchange) as ZXReturnsExchange, SUM(ZXReturnsRefund) as ZXReturnsRefund, SUM(ZXTotalQTYSold) as ZXTotalQTYSold, 
    SUM(ZXTotalTransactionCount) as ZXTotalTransactionCount, SUM(ZXTotalGuess) as ZXTotalGuess, SUM(ZXCurrentTotalSales) as ZXCurrentTotalSales,
    SUM(ZXAddOns) as ZXAddOns, SUM(ZXFamousBlends) as ZXFamousBlends, SUM(ZXCombo) as ZXCombo, 
    SUM(ZXPerfectCombination) as ZXPerfectCombination, SUM(ZXPremium) as ZXPremium, SUM(ZXSavoury) as ZXSavoury, SUM(ZXSimplyPerfect) as ZXSimplyPerfect, 
    SUM(ZXThousandQty) as ZXThousandQty, SUM(ZXFiveHundredQty) as ZXFiveHundredQty, SUM(ZXTwoHundredQty) as ZXTwoHundredQty, SUM(ZXOneHundredQty) as ZXOneHundredQty, 
    SUM(ZXFiftyQty) as ZXFiftyQty, SUM(ZXTwentyQty) as ZXTwentyQty, SUM(ZXTenQty) as ZXTenQty, SUM(ZXFiveQty) as ZXFiveQty, SUM(ZXOneQty) as ZXOneQty, 
    SUM(ZXPointTwentyFiveQty) as ZXPointTwentyFiveQty, SUM(ZXPointFiveQty) as ZXPointFiveQty, SUM(ZXThousandTotal) as ZXThousandTotal, SUM(ZXFiveHundredTotal) as ZXFiveHundredTotal, 
    SUM(ZXTwoHundredTotal) as ZXTwoHundredTotal, SUM(ZXOneHundredTotal) as ZXOneHundredTotal, SUM(ZXFiftyTotal) as ZXFiftyTotal, 
    SUM(ZXTwentyTotal) as ZXTwentyTotal, SUM(ZXTenTotal) as ZXTenTotal, SUM(ZXFiveTotal) as ZXFiveTotal, SUM(ZXOneTotal) as ZXOneTotal, SUM(ZXPointTwentyFiveTotal) as ZXPointTwentyFiveTotal, 
    SUM(ZXPointFiveTotal) as ZXPointFiveTotal, SUM(ZXdate) as ZXdate, SUM(ZXCBQtyTotal) as ZXCBQtyTotal, SUM(ZXCBGrandTotal) as ZXCBGrandTotal"
    Public Sub FillReportValue(DateFrom As String, DateTo As String)

        Dim ConnectionLocal As MySqlConnection = LocalhostConn()
        Dim Query As String = $"SELECT {zrpZreadTable} FROM loc_zread_table WHERE ZXdate >= '{DateFrom}' AND ZXdate <= '{DateTo}' AND status = 1"
        Dim Command As New MySqlCommand(Query, ConnectionLocal)

        Using reader As MySqlDataReader = Command.ExecuteReader
            If reader.HasRows Then
                While reader.Read

                    zrpTerminal = If(Not IsDBNull(reader("ZXTerminal")), reader("ZXTerminal"), 0)
                    zrpGrossSales = If(Not IsDBNull(reader("ZXGross")), reader("ZXGross"), 0)
                    zrpLessVatVE = If(Not IsDBNull(reader("ZXLessVat")), reader("ZXLessVat"), 0)
                    zrpLessVatDiplomat = If(Not IsDBNull(reader("ZXLessVatDiplomat")), reader("ZXLessVatDiplomat"), 0)
                    zrpLessVatOther = If(Not IsDBNull(reader("ZXLessVatOthers")), reader("ZXLessVatOthers"), 0)
                    zrpAddVat = If(Not IsDBNull(reader("ZXAddVat")), reader("ZXAddVat"), 0)
                    zrpDailySales = If(Not IsDBNull(reader("ZXDailySales")), reader("ZXDailySales"), 0)

                    zrpVatAmount = If(Not IsDBNull(reader("ZXVatAmount")), reader("ZXVatAmount"), 0)
                    zrpGovTax = If(Not IsDBNull(reader("ZXLocalGovTax")), reader("ZXLocalGovTax"), 0)
                    zrpVatableSales = If(Not IsDBNull(reader("ZXVatableSales")), reader("ZXVatableSales"), 0)
                    zrpZeroRatedSales = If(Not IsDBNull(reader("ZXZeroRatedSales")), reader("ZXZeroRatedSales"), 0)
                    zrpVatExempt = If(Not IsDBNull(reader("ZXVatExemptSales")), reader("ZXVatExemptSales"), 0)
                    zrpLessDiscVE = If(Not IsDBNull(reader("ZXLessDiscVE")), reader("ZXLessDiscVE"), 0)
                    zrpNetSales = If(Not IsDBNull(reader("ZXNetSales")), reader("ZXNetSales"), 0)

                    zrpCashTotal = If(Not IsDBNull(reader("ZXCashTotal")), reader("ZXCashTotal"), 0)
                    zrpCreditCard = If(Not IsDBNull(reader("ZXCreditCard")), reader("ZXCreditCard"), 0)
                    zrpDebitCard = If(Not IsDBNull(reader("ZXDebitCard")), reader("ZXDebitCard"), 0)
                    zrpMisc = If(Not IsDBNull(reader("ZXMiscCheques")), reader("ZXMiscCheques"), 0)
                    zrpGC = If(Not IsDBNull(reader("ZXGiftCard")), reader("ZXGiftCard"), 0)
                    zrpGCUsed = If(Not IsDBNull(reader("ZXGiftCardSum")), reader("ZXGiftCardSum"), 0)
                    zrpAR = If(Not IsDBNull(reader("ZXAR")), reader("ZXAR"), 0)
                    zrpTotalExpenses = If(Not IsDBNull(reader("ZXTotalExpenses")), reader("ZXTotalExpenses"), 0)
                    zrpOthers = If(Not IsDBNull(reader("ZXCardOthers")), reader("ZXCardOthers"), 0)
                    zrpCompBegBalance = If(Not IsDBNull(reader("ZXCompBegBal")), reader("ZXCompBegBal"), 0)
                    zrpDeposit = If(Not IsDBNull(reader("ZXDeposits")), reader("ZXDeposits"), 0)
                    zrpCashinDrawer = If(Not IsDBNull(reader("ZXCashInDrawer")), reader("ZXCashInDrawer"), 0)

                    zrpCashless = If(Not IsDBNull(reader("ZXCashlessTotal")), reader("ZXCashlessTotal"), 0)
                    zrpGcash = If(Not IsDBNull(reader("ZXGcash")), reader("ZXGcash"), 0)
                    zrpPaymaya = If(Not IsDBNull(reader("ZXPaymaya")), reader("ZXPaymaya"), 0)
                    zrpShopee = If(Not IsDBNull(reader("ZXShopeePay")), reader("ZXShopeePay"), 0)
                    zrpFoodPanda = If(Not IsDBNull(reader("ZXFoodPanda")), reader("ZXFoodPanda"), 0)
                    zrpGrab = If(Not IsDBNull(reader("ZXGrabFood")), reader("ZXGrabFood"), 0)
                    zrpCompExpenses = If(Not IsDBNull(reader("ZXRepExpense")), reader("ZXRepExpense"), 0)
                    zrpCashlessOthers = If(Not IsDBNull(reader("ZXCashlessOthers")), reader("ZXCashlessOthers"), 0)

                    zrpItemVoid = If(Not IsDBNull(reader("ZXItemVoidEC")), reader("ZXItemVoidEC"), 0)
                    zrpTrxVoid = If(Not IsDBNull(reader("ZXTransactionVoid")), reader("ZXTransactionVoid"), 0)
                    zrpTrxCancel = If(Not IsDBNull(reader("ZXTransactionCancel")), reader("ZXTransactionCancel"), 0)
                    zrpDiplomat = If(Not IsDBNull(reader("ZXDiplomat")), reader("ZXDiplomat"), 0)
                    zrpTotalDisc = If(Not IsDBNull(reader("ZXTotalDiscounts")), reader("ZXTotalDiscounts"), 0)
                    zrpSeniorDisc = If(Not IsDBNull(reader("ZXSeniorCitizen")), reader("ZXSeniorCitizen"), 0)
                    zrpPWDDisc = If(Not IsDBNull(reader("ZXPWD")), reader("ZXPWD"), 0)
                    zrpAthleteDisc = If(Not IsDBNull(reader("ZXAthlete")), reader("ZXAthlete"), 0)
                    zrpSPDisc = If(Not IsDBNull(reader("ZXSingleParent")), reader("ZXSingleParent"), 0)
                    zrpDiscOthers = If(Not IsDBNull(reader("ZXDiscOthers")), reader("ZXDiscOthers"), 0)

                    zrpTakeOutCharge = If(Not IsDBNull(reader("ZXTakeOutCharge")), reader("ZXTakeOutCharge"), 0)
                    zrpDelCharge = If(Not IsDBNull(reader("ZXDeliveryCharge")), reader("ZXDeliveryCharge"), 0)
                    zrpReturnEx = If(Not IsDBNull(reader("ZXReturnsExchange")), reader("ZXReturnsExchange"), 0)
                    zrpReturnRef = If(Not IsDBNull(reader("ZXReturnsRefund")), reader("ZXReturnsRefund"), 0)
                    zrpTotalQTYSold = If(Not IsDBNull(reader("ZXTotalQTYSold")), reader("ZXTotalQTYSold"), 0)
                    zrpTotalTrxCount = If(Not IsDBNull(reader("ZXTotalTransactionCount")), reader("ZXTotalTransactionCount"), 0)
                    zrpTotalGuest = If(Not IsDBNull(reader("ZXTotalGuess")), reader("ZXTotalGuess"), 0)
                    zrpCurrentTotalSales = If(Not IsDBNull(reader("ZXCurrentTotalSales")), reader("ZXCurrentTotalSales"), 0)



                    zrpAddOns = If(Not IsDBNull(reader("ZXAddOns")), reader("ZXAddOns"), 0)
                    zrpFamousBlends = If(Not IsDBNull(reader("ZXFamousBlends")), reader("ZXFamousBlends"), 0)
                    zrpCombo = If(Not IsDBNull(reader("ZXCombo")), reader("ZXCombo"), 0)
                    zrpPerfectC = If(Not IsDBNull(reader("ZXPerfectCombination")), reader("ZXPerfectCombination"), 0)
                    zrpPremium = If(Not IsDBNull(reader("ZXPremium")), reader("ZXPremium"), 0)
                    zrpSavory = If(Not IsDBNull(reader("ZXSavoury")), reader("ZXSavoury"), 0)
                    zrpSimplyP = If(Not IsDBNull(reader("ZXSimplyPerfect")), reader("ZXSimplyPerfect"), 0)

                    zrpThousandQty = If(Not IsDBNull(reader("ZXThousandQty")), reader("ZXThousandQty"), 0)
                    zrpFiveHundredQty = If(Not IsDBNull(reader("ZXFiveHundredQty")), reader("ZXFiveHundredQty"), 0)
                    zrpTwoHundredQty = If(Not IsDBNull(reader("ZXTwoHundredQty")), reader("ZXTwoHundredQty"), 0)
                    zrpOneHundredQty = If(Not IsDBNull(reader("ZXOneHundredQty")), reader("ZXOneHundredQty"), 0)
                    zrpFiftyQty = If(Not IsDBNull(reader("ZXFiftyQty")), reader("ZXFiftyQty"), 0)
                    zrpTwentyQty = If(Not IsDBNull(reader("ZXTwentyQty")), reader("ZXTwentyQty"), 0)
                    zrpTenQty = If(Not IsDBNull(reader("ZXTenQty")), reader("ZXTenQty"), 0)
                    zrpFiveQty = If(Not IsDBNull(reader("ZXFiveQty")), reader("ZXFiveQty"), 0)
                    zrpOneQty = If(Not IsDBNull(reader("ZXOneQty")), reader("ZXOneQty"), 0)
                    zrpPointTwentyFiveQty = If(Not IsDBNull(reader("ZXPointTwentyFiveQty")), reader("ZXPointTwentyFiveQty"), 0)
                    zrpPointFiveQty = If(Not IsDBNull(reader("ZXPointFiveQty")), reader("ZXPointFiveQty"), 0)

                    zrpThousandTotal = If(Not IsDBNull(reader("ZXThousandTotal")), reader("ZXThousandTotal"), 0)
                    zrpFiveHundredTotal = If(Not IsDBNull(reader("ZXFiveHundredTotal")), reader("ZXFiveHundredTotal"), 0)
                    zrpTwoHundredTotal = If(Not IsDBNull(reader("ZXTwoHundredTotal")), reader("ZXTwoHundredTotal"), 0)
                    zrpOneHundredTotal = If(Not IsDBNull(reader("ZXOneHundredTotal")), reader("ZXOneHundredTotal"), 0)
                    zrpFiftyTotal = If(Not IsDBNull(reader("ZXFiftyTotal")), reader("ZXFiftyTotal"), 0)
                    zrpTwentyTotal = If(Not IsDBNull(reader("ZXTwentyTotal")), reader("ZXTwentyTotal"), 0)
                    zrpTenTotal = If(Not IsDBNull(reader("ZXTenTotal")), reader("ZXTenTotal"), 0)
                    zrpFiveTotal = If(Not IsDBNull(reader("ZXFiveTotal")), reader("ZXFiveTotal"), 0)

                    zrpOneTotal = If(Not IsDBNull(reader("ZXOneTotal")), reader("ZXOneTotal"), 0)
                    zrpPointTwentyFiveTotal = If(Not IsDBNull(reader("ZXPointTwentyFiveTotal")), reader("ZXPointTwentyFiveTotal"), 0)
                    zrpPointFiveTotal = If(Not IsDBNull(reader("ZXPointFiveTotal")), reader("ZXPointFiveTotal"), 0)
                    zrpCBQTYTotal = If(Not IsDBNull(reader("ZXCBQtyTotal")), reader("ZXCBQtyTotal"), 0)
                    zrpCBGrandTotal = If(Not IsDBNull(reader("ZXCBGrandTotal")), reader("ZXCBGrandTotal"), 0)

                End While
            End If
        End Using

        zrpBegSINo = returnselect("ZXBegSINo", "`loc_zread_table` WHERE ZXdate = '" & DateFrom & "' AND status = 1 Order by id ASC limit 1")
        zrpEndSINo = returnselect("ZXEndSINo", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")

        zrpBegTrxNo = returnselect("ZXBegTransNo", "`loc_zread_table` WHERE ZXdate = '" & DateFrom & "' AND status = 1 Order by id ASC limit 1")
        zrpEndTrxNo = returnselect("ZXEndTransNo", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")

        zrpBegBalance = ReturnRowDouble("ZXBegBalance", "`loc_zread_table` WHERE ZXdate = '" & DateFrom & "' AND status = 1 Order by id ASC limit 1")
        zrpEndBalance = ReturnRowDouble("ZXEndingBalance", "`loc_zread_table` WHERE ZXdate = '" & DateFrom & "' AND status = 1 Order by id ASC limit 1")

        zrpAccumulatedGTS = ReturnRowDouble("ZXAccumulatedGT", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")
        zrpResetCounter = ReturnRowDouble("ZXResetCounter", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")

        zrpZCounter = ReturnRowDouble("ZXZCounter", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")

        zrpReprintCount = ReturnRowDouble("ZXReprintCount", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")


        Dim zreadDtFooterFrom = returnselect("ZXDateFooter", "`loc_zread_table` WHERE ZXdate = '" & DateFrom & "' AND status = 1 Order by id ASC limit 1")
        Dim zreadDtFooterTo = returnselect("ZXDateFooter", "`loc_zread_table` WHERE ZXdate = '" & DateTo & "' AND status = 1 Order by id ASC limit 1")

        If zreadDtFooterFrom <> "" Then
            Dim zreadDtFrom As DateTime = DateTime.ParseExact(zreadDtFooterFrom, "yyyy-MM-dd HH:mm:ss", Nothing)
            zrpDateFooterFrom = zreadDtFrom.ToString("yyyy-MM-dd")
        Else
            zrpDateFooterFrom = DateFrom
        End If

        If zreadDtFooterTo <> "" Then
            Dim zreadDtTo As DateTime = DateTime.ParseExact(zreadDtFooterTo, "yyyy-MM-dd HH:mm:ss", Nothing)
            zrpDateFooterTo = zreadDtTo.ToString("yyyy-MM-dd")
        Else
            zrpDateFooterTo = DateTo
        End If

    End Sub
    Public Sub zreadRepBody(sender As Object, e As PrintPageEventArgs)
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
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TERMINAL NO.", zrpTerminal, FontDefault, 5, 0)
            FillEJournalContent("TERMINAL NO.          " & zrpTerminal, {"TERMINAL NO.", zrpTerminal}, "LR", True, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GROSS", NUMBERFORMAT(zrpGrossSales), FontDefault, 5, 0)
            FillEJournalContent("GROSS         " & NUMBERFORMAT(zrpGrossSales), {"GROSS", NUMBERFORMAT(zrpGrossSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (VE)", NUMBERFORMAT(zrpLessVatVE), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (VE)         " & NUMBERFORMAT(zrpLessVatVE), {"LESS VAT (VE)", NUMBERFORMAT(zrpLessVatVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT DIPLOMAT", NUMBERFORMAT(zrpLessVatDiplomat), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT DIPLOMAT         " & NUMBERFORMAT(zrpLessVatDiplomat), {"LESS VAT DIPLOMAT", NUMBERFORMAT(zrpLessVatDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS VAT (OTHER)", NUMBERFORMAT(zrpLessVatOther), FontDefault, 5, 0)
            FillEJournalContent("LESS VAT (OTHER)          " & NUMBERFORMAT(zrpLessVatOther), {"LESS VAT (OTHER)", NUMBERFORMAT(zrpLessVatOther)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD VAT", NUMBERFORMAT(zrpAddVat), FontDefault, 5, 0)
            FillEJournalContent("ADD VAT          " & NUMBERFORMAT(zrpAddVat), {"ADD VAT", NUMBERFORMAT(zrpAddVat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DAILY SALES", NUMBERFORMAT(zrpDailySales), FontDefault, 5, 0)
            FillEJournalContent("DAILY SALES          " & NUMBERFORMAT(zrpDailySales), {"DAILY SALES", NUMBERFORMAT(zrpDailySales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT AMOUNT", NUMBERFORMAT(zrpVatAmount), FontDefault, 5, 0)
            FillEJournalContent("VAT AMOUNT          " & NUMBERFORMAT(zrpVatAmount), {"VAT AMOUNT", NUMBERFORMAT(zrpVatAmount)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LOCAL GOV'T TAX", NUMBERFORMAT(zrpGovTax), FontDefault, 5, 0)
            FillEJournalContent("LOCAL GOV'T TAX          " & NUMBERFORMAT(zrpGovTax), {"LOCAL GOV'T TAX", NUMBERFORMAT(zrpGovTax)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VATABLE SALES", NUMBERFORMAT(zrpVatableSales), FontDefault, 5, 0)
            FillEJournalContent("VATABLE SALES          " & NUMBERFORMAT(zrpVatableSales), {"VATABLE SALES", NUMBERFORMAT(zrpVatableSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ZERO RATED SALES", NUMBERFORMAT(zrpZeroRatedSales), FontDefault, 5, 0)
            FillEJournalContent("ZERO RATED SALES          " & NUMBERFORMAT(zrpZeroRatedSales), {"ZERO RATED SALES", NUMBERFORMAT(zrpZeroRatedSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "VAT EXEMPT SALES", NUMBERFORMAT(zrpVatExempt), FontDefault, 5, 0)
            FillEJournalContent("VAT EXEMPT SALES          " & NUMBERFORMAT(zrpVatExempt), {"VAT EXEMPT SALES", NUMBERFORMAT(zrpVatExempt)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "LESS DISC (VE)", NUMBERFORMAT(zrpLessDiscVE), FontDefault, 5, 0)
            FillEJournalContent("LESS DISC (VE)          " & NUMBERFORMAT(zrpLessDiscVE), {"LESS DISC (VE)", NUMBERFORMAT(zrpLessDiscVE)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "NET SALES", NUMBERFORMAT(zrpNetSales), FontDefault, 5, 0)
            FillEJournalContent("NET SALES          " & NUMBERFORMAT(zrpNetSales), {"NET SALES", NUMBERFORMAT(zrpNetSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 20

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH TOTAL", NUMBERFORMAT(zrpCashTotal), FontDefault, 5, 0)
            FillEJournalContent("CASH TOTAL          " & NUMBERFORMAT(zrpCashTotal), {"CASH TOTAL", NUMBERFORMAT(zrpCashTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CREDIT CARD", NUMBERFORMAT(zrpCreditCard), FontDefault, 5, 0)
            FillEJournalContent("CREDIT CARD          " & NUMBERFORMAT(zrpCreditCard), {"CREDIT CARD", NUMBERFORMAT(zrpCreditCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEBIT CARD", NUMBERFORMAT(zrpDebitCard), FontDefault, 5, 0)
            FillEJournalContent("DEBIT CARD          " & NUMBERFORMAT(zrpDebitCard), {"DEBIT CARD", NUMBERFORMAT(zrpDebitCard)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "MISC/CHEQUES", NUMBERFORMAT(zrpMisc), FontDefault, 5, 0)
            FillEJournalContent("MISC/CHEQUES          " & NUMBERFORMAT(zrpDebitCard), {"MISC/CHEQUES", NUMBERFORMAT(zrpMisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD(GC)", NUMBERFORMAT(zrpGC), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD(GC)          " & NUMBERFORMAT(zrpGC), {"GIFT CARD(GC)", NUMBERFORMAT(zrpGC)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GIFT CARD USED", NUMBERFORMAT(zrpGCUsed), FontDefault, 5, 0)
            FillEJournalContent("GIFT CARD SUM          " & NUMBERFORMAT(zrpGCUsed), {"GIFT CARD SUM", NUMBERFORMAT(zrpGCUsed)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "A/R", NUMBERFORMAT(zrpAR), FontDefault, 5, 0)
            FillEJournalContent("A/R          " & NUMBERFORMAT(zrpAR), {"A/R", NUMBERFORMAT(zrpAR)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL EXPENSES", NUMBERFORMAT(zrpTotalExpenses), FontDefault, 5, 0)
            FillEJournalContent("TOTAL EXPENSES         " & NUMBERFORMAT(zrpTotalExpenses), {"TOTAL EXPENSES", NUMBERFORMAT(zrpTotalExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", "N/A", FontDefault, 5, 0)
            FillEJournalContent("OTHERS         N/A", {"OTHERS", "N/A"}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEG.BALANCE", NUMBERFORMAT(Double.Parse(zrpCompBegBalance)), FontDefault, 5, 0)
            FillEJournalContent("BEG.BALANCE         " & NUMBERFORMAT(Double.Parse(zrpCompBegBalance)), {"BEG.BALANCE", NUMBERFORMAT(Double.Parse(zrpCompBegBalance))}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DEPOSIT", NUMBERFORMAT(zrpDeposit), FontDefault, 5, 0)
            FillEJournalContent("DEPOSIT          " & NUMBERFORMAT(zrpDeposit), {"DEPOSIT", NUMBERFORMAT(zrpDeposit)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASH IN DRAWER", NUMBERFORMAT(zrpCashinDrawer), FontDefault, 5, 0)
            FillEJournalContent("CASH IN DRAWER          " & NUMBERFORMAT(zrpCashinDrawer), {"CASH IN DRAWER", NUMBERFORMAT(zrpCashinDrawer)}, "LR", False, False)
            RECEIPTLINECOUNT += 20
            FillEJournalContent("", {}, "C", True, True)
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CASHLESS", NUMBERFORMAT(zrpCashless), FontDefault, 5, 0)
            FillEJournalContent("CASHLESS          " & NUMBERFORMAT(zrpCashless), {"CASHLESS", NUMBERFORMAT(zrpCashless)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GCASH", NUMBERFORMAT(zrpGcash), FontDefault, 5, 0)
            FillEJournalContent("GCASH          " & NUMBERFORMAT(zrpGcash), {"GCASH", NUMBERFORMAT(zrpGcash)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PAYMAYA", NUMBERFORMAT(zrpPaymaya), FontDefault, 5, 0)
            FillEJournalContent("PAYMAYA          " & NUMBERFORMAT(zrpPaymaya), {"PAYMAYA", NUMBERFORMAT(zrpPaymaya)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SHOPEE", NUMBERFORMAT(zrpShopee), FontDefault, 5, 0)
            FillEJournalContent("SHOPEE          " & NUMBERFORMAT(zrpShopee), {"SHOPEE", NUMBERFORMAT(zrpShopee)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FOODPANDA", NUMBERFORMAT(zrpFoodPanda), FontDefault, 5, 0)
            FillEJournalContent("FOODPANDA          " & NUMBERFORMAT(zrpFoodPanda), {"FOODPANDA", NUMBERFORMAT(zrpFoodPanda)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAB", NUMBERFORMAT(zrpGrab), FontDefault, 5, 0)
            FillEJournalContent("GRAB          " & NUMBERFORMAT(zrpGrab), {"GRAB", NUMBERFORMAT(zrpGrab)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMPLIMENTARY", NUMBERFORMAT(zrpCompExpenses), FontDefault, 5, 0)
            FillEJournalContent("COMPLIMENTARY          " & NUMBERFORMAT(zrpCompExpenses), {"COMPLIMENTARY", NUMBERFORMAT(zrpCompExpenses)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "OTHERS", NUMBERFORMAT(zrpCashlessOthers), FontDefault, 5, 0)
            FillEJournalContent("OTHERS          " & NUMBERFORMAT(zrpCashlessOthers), {"OTHERS", NUMBERFORMAT(zrpCashlessOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ITEM VOID E/C", NUMBERFORMAT(zrpItemVoid), FontDefault, 5, 0)
            FillEJournalContent("ITEM VOID E/C          " & NUMBERFORMAT(zrpItemVoid), {"ITEM VOID E/C", NUMBERFORMAT(zrpItemVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION VOID", NUMBERFORMAT(zrpTrxVoid), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION VOID         " & NUMBERFORMAT(zrpTrxVoid), {"TRANSACTION VOID", NUMBERFORMAT(zrpTrxVoid)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TRANSACTION CANCEL", NUMBERFORMAT(zrpTrxCancel), FontDefault, 5, 0)
            FillEJournalContent("TRANSACTION CANCEL         " & NUMBERFORMAT(zrpTrxCancel), {"TRANSACTION CANCEL", NUMBERFORMAT(zrpTrxCancel)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DIMPLOMAT", NUMBERFORMAT(zrpDiplomat), FontDefault, 5, 0)
            FillEJournalContent("DIMPLOMAT         " & NUMBERFORMAT(zrpDiplomat), {"DIMPLOMAT", NUMBERFORMAT(zrpDiplomat)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL DISCOUNTS", NUMBERFORMAT(zrpTotalDisc), FontDefault, 5, 0)
            FillEJournalContent("TOTAL DISCOUNTS         " & NUMBERFORMAT(zrpTotalDisc), {"TOTAL DISCOUNTS", NUMBERFORMAT(zrpTotalDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - SENIOR CITIZEN", NUMBERFORMAT(zrpSeniorDisc), FontDefault, 5, 0)
            FillEJournalContent(" - SENIOR CITIZEN         " & NUMBERFORMAT(zrpSeniorDisc), {" - SENIOR CITIZEN", NUMBERFORMAT(zrpSeniorDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - PWD", NUMBERFORMAT(zrpPWDDisc), FontDefault, 5, 0)
            FillEJournalContent(" - PWD         " & NUMBERFORMAT(zrpPWDDisc), {" -PWD", NUMBERFORMAT(zrpPWDDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - ATHLETE", NUMBERFORMAT(zrpAthleteDisc), FontDefault, 5, 0)
            FillEJournalContent(" - ATHLETE         " & NUMBERFORMAT(zrpAthleteDisc), {" - ATHLETE", NUMBERFORMAT(zrpAthleteDisc)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, " - OTHERS", NUMBERFORMAT(zrpDiscOthers), FontDefault, 5, 0)
            FillEJournalContent(" - OTHERS         " & NUMBERFORMAT(zrpDiscOthers), {" - OTHERS", NUMBERFORMAT(zrpDiscOthers)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TAKE OUT CHARGE", NUMBERFORMAT(zrpTakeOutCharge), FontDefault, 5, 0)
            FillEJournalContent("TAKE OUT CHARGE         " & NUMBERFORMAT(zrpTakeOutCharge), {"TAKE OUT CHARGE", NUMBERFORMAT(zrpTakeOutCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "DELIVERY CHARGE", NUMBERFORMAT(zrpDelCharge), FontDefault, 5, 0)
            FillEJournalContent("DELIVERY CHARGE         " & NUMBERFORMAT(zrpDelCharge), {"DELIVERY CHARGE", NUMBERFORMAT(zrpDelCharge)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS EXCHANGE", NUMBERFORMAT(zrpReturnEx), FontDefault, 5, 0)
            FillEJournalContent("RETURNS EXCHANGE         " & NUMBERFORMAT(zrpReturnEx), {"RETURNS EXCHANGE", NUMBERFORMAT(zrpReturnEx)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RETURNS REFUND", NUMBERFORMAT(zrpReturnRef), FontDefault, 5, 0)
            FillEJournalContent("RETURNS REFUND         " & NUMBERFORMAT(zrpReturnRef), {"RETURNS REFUND", NUMBERFORMAT(zrpReturnRef)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL QTY SOLD", zrpTotalQTYSold, FontDefault, 5, 0)
            FillEJournalContent("TOTAL QTY SOLD         " & zrpTotalQTYSold, {"TOTAL QTY SOLD", zrpTotalQTYSold}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL TRANS. COUNT", zrpTotalTrxCount, FontDefault, 5, 0)
            FillEJournalContent("TOTAL TRANS. COUNT         " & zrpTotalTrxCount, {"TOTAL TRANS. COUNT", zrpTotalTrxCount}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "TOTAL GUEST", zrpTotalGuest, FontDefault, 5, 0)
            FillEJournalContent("TOTAL GUEST         " & zrpTotalGuest, {"TOTAL GUEST", zrpTotalGuest}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING S.I NO.", zrpBegSINo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING S.I NO.         " & zrpBegSINo, {"BEGINNING S.I NO.", zrpBegSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END S.I NO.", zrpEndSINo, FontDefault, 5, 0)
            FillEJournalContent("END S.I NO.         " & zrpEndSINo, {"END S.I NO.", zrpEndSINo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING TRANS. NO.", zrpBegTrxNo, FontDefault, 5, 0)
            FillEJournalContent("BEGINNING TRANS. NO.         " & zrpBegTrxNo, {"BEGINNING TRANS. NO.", zrpBegTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "END TRANS. NO.", zrpEndTrxNo, FontDefault, 5, 0)
            FillEJournalContent("END TRANS. NO.         " & zrpEndTrxNo, {"END TRANS. NO.", zrpEndTrxNo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "CURRENT TOTAL SALES", NUMBERFORMAT(zrpCurrentTotalSales), FontDefault, 5, 0)
            FillEJournalContent("CURRENT TOTAL SALES         " & NUMBERFORMAT(zrpCurrentTotalSales), {"CURRENT TOTAL SALES", NUMBERFORMAT(zrpCurrentTotalSales)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "BEGINNING BALANCE", NUMBERFORMAT(zrpBegBalance), FontDefault, 5, 0)
            FillEJournalContent("BEGINNING BALANCE         " & NUMBERFORMAT(zrpBegBalance), {"BEGINNING BALANCE", NUMBERFORMAT(zrpBegBalance)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ENDING BALANCE", NUMBERFORMAT(zrpEndBalance), FontDefault, 5, 0)
            FillEJournalContent("ENDING BALANCE         " & NUMBERFORMAT(zrpEndBalance), {"ENDING BALANCE", NUMBERFORMAT(zrpEndBalance)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ACCUMULATED GRAND TOTAL SALES", NUMBERFORMAT(zrpAccumulatedGTS), FontDefault, 5, 0)
            FillEJournalContent("ACCUMULATED GRAND TOTAL SALES         " & NUMBERFORMAT(zrpAccumulatedGTS), {"ACCUMULATED GRAND TOTAL SALES", NUMBERFORMAT(zrpAccumulatedGTS)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RESET COUNTER", NUMBERFORMAT(zrpResetCounter), FontDefault, 5, 0)
            FillEJournalContent("RESET COUNTER         " & NUMBERFORMAT(zrpResetCounter), {"RESET COUNTER", NUMBERFORMAT(zrpResetCounter)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "Z-COUNTER", NUMBERFORMAT(zrpZCounter), FontDefault, 5, 0)
            FillEJournalContent("Z-COUNTER         " & NUMBERFORMAT(zrpZCounter), {"Z-COUNTER", NUMBERFORMAT(zrpZCounter)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "RE-PRINT COUNT", NUMBERFORMAT(zrpReprintCount), FontDefault, 5, 0)
            FillEJournalContent("RE-PRINT COUNT         " & NUMBERFORMAT(zrpReprintCount), {"RE-PRINT COUNT", NUMBERFORMAT(zrpReprintCount)}, "LR", False, False)
            RECEIPTLINECOUNT += 10

            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 10
            SimpleTextDisplay(sender, e, "SALES BY CLASS", FontDefault, 0, RECEIPTLINECOUNT)
            FillEJournalContent("SALES BY CLASS", {}, "S", False, True)
            RECEIPTLINECOUNT += 30
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "ADD ONS", zrpAddOns, FontDefault, 5, 0)
            FillEJournalContent("ADD ONS          " & zrpAddOns, {"ADD ONS", zrpAddOns}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "FAMOUS BLENDS", zrpFamousBlends, FontDefault, 5, 0)
            FillEJournalContent("FAMOUS BLENDS          " & zrpFamousBlends, {"FAMOUS BLENDS", zrpFamousBlends}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "COMBO", zrpCombo, FontDefault, 5, 0)
            FillEJournalContent("COMBO          " & zrpCombo, {"COMBO", zrpCombo}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PERFECT COMBINATION", zrpPerfectC, FontDefault, 5, 0)
            FillEJournalContent("PERFECT COMBINATION          " & zrpPerfectC, {"PERFECT COMBINATION", zrpPerfectC}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "PREMIUM LINE", zrpPremium, FontDefault, 5, 0)
            FillEJournalContent("PREMIUM LINE          " & zrpPremium, {"PREMIUM LINE", zrpPremium}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SAVORY", zrpSavory, FontDefault, 5, 0)
            FillEJournalContent("SAVORY          " & zrpSavory, {"SAVORY", zrpSavory}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "SIMPY PERFECT", zrpSimplyP, FontDefault, 5, 0)
            FillEJournalContent("SIMPY PERFECT          " & zrpSimplyP, {"SIMPY PERFECT", zrpSimplyP}, "LR", False, False)
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
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1000", NUMBERFORMAT(zrpThousandTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpThousandQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1000           " & zrpThousandQty & "           " & NUMBERFORMAT(zrpThousandTotal), {"₱ 1000", zrpThousandQty, NUMBERFORMAT(zrpThousandTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 500", NUMBERFORMAT(zrpFiveHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpFiveHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 500           " & zrpFiveHundredQty & "           " & NUMBERFORMAT(zrpFiveHundredTotal), {"₱ 500", zrpFiveHundredQty, NUMBERFORMAT(zrpFiveHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 200", NUMBERFORMAT(zrpTwoHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpTwoHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 200           " & zrpTwoHundredQty & "           " & NUMBERFORMAT(zrpTwoHundredTotal), {"₱ 200", zrpTwoHundredQty, NUMBERFORMAT(zrpTwoHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 100", NUMBERFORMAT(zrpOneHundredTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpOneHundredQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 100           " & zrpOneHundredQty & "           " & NUMBERFORMAT(zrpOneHundredTotal), {"₱ 100", zrpOneHundredQty, NUMBERFORMAT(zrpOneHundredTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 50", NUMBERFORMAT(zrpFiftyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpFiftyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 50           " & zrpFiftyQty & "           " & NUMBERFORMAT(zrpFiftyTotal), {"₱ 50", zrpFiftyQty, NUMBERFORMAT(zrpFiftyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 20", NUMBERFORMAT(zrpTwentyTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpTwentyQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 20           " & zrpTwentyQty & "           " & NUMBERFORMAT(zrpTwentyTotal), {"₱ 20", zrpTwentyQty, NUMBERFORMAT(zrpTwentyTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 10", NUMBERFORMAT(zrpTenTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpTenQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 10           " & zrpTenQty & "           " & NUMBERFORMAT(zrpTenTotal), {"₱ 10", zrpTenQty, NUMBERFORMAT(zrpTenTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 5", NUMBERFORMAT(zrpFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 5           " & zrpFiveQty & "           " & NUMBERFORMAT(zrpFiveTotal), {"₱ 5", zrpFiveQty, NUMBERFORMAT(zrpFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ 1", NUMBERFORMAT(zrpOneTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpOneQty, FontDefault, 0, 110)
            FillEJournalContent("₱ 1           " & zrpOneQty & "           " & NUMBERFORMAT(zrpOneTotal), {"₱ 1", zrpOneQty, NUMBERFORMAT(zrpOneTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .25", NUMBERFORMAT(zrpPointTwentyFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpPointTwentyFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .25           " & zrpPointTwentyFiveQty & "           " & NUMBERFORMAT(zrpPointTwentyFiveTotal), {"₱ .25", zrpPointTwentyFiveQty, NUMBERFORMAT(zrpPointTwentyFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT += 10
            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "₱ .05", NUMBERFORMAT(zrpPointFiveTotal), FontDefault, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpPointFiveQty, FontDefault, 0, 110)
            FillEJournalContent("₱ .05           " & zrpPointFiveQty & "           " & NUMBERFORMAT(zrpPointFiveTotal), {"₱ .05", zrpPointFiveQty, NUMBERFORMAT(zrpPointFiveTotal)}, "S3", False, False)
            RECEIPTLINECOUNT -= 10
            PrintSmallLine(sender, e, FontDefaultLine, RECEIPTLINECOUNT)
            FillEJournalContent("------------------------------------------------------------", {}, "C", True, True)
            RECEIPTLINECOUNT += 30

            RightToLeftDisplay(sender, e, RECEIPTLINECOUNT, "GRAND TOTAL", NUMBERFORMAT(zrpCBGrandTotal), FontDefaultBold, 5, 0)
            RightDisplay1(sender, e, RECEIPTLINECOUNT, "", zrpCBQTYTotal, FontDefaultBold, 0, 110)
            FillEJournalContent("GRAND TOTAL          " & NUMBERFORMAT(zrpCBGrandTotal), {"GRAND TOTAL", NUMBERFORMAT(zrpCBGrandTotal)}, "LR", False, False)
            RECEIPTLINECOUNT += 10
            CenterTextDisplay(sender, e, zrpDateFooterFrom & " - " & zrpDateFooterTo, FontDefault, RECEIPTLINECOUNT)
            FillEJournalContent(zrpDateFooterFrom & " - " & zrpDateFooterTo, {}, "C", False, True)

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Public Sub UpdateReprintCount(FromDate, ToDate)
        If FromDate = ToDate Then
            Dim ConnectionLocal As MySqlConnection = LocalhostConn()
            Dim Query As String = "UPDATE loc_zread_table SET ZXReprintCount = @1 WHERE ZXdate = '" & FromDate & "'"
            Dim Command As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            zrpReprintCount += 1
            Command.Parameters.Add("@1", MySqlDbType.Text).Value = zrpReprintCount
            Command.ExecuteNonQuery()
        End If
    End Sub
End Class
