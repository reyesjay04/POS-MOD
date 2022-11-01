Public Class SyncCls
    Public Class CategorySync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class InventorySync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class FormulaSync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class CouponSync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class PartnersSync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class StockCatSync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
    Public Class ProductsSync
        Property lastFetchID As Integer = 0 'Get last id of fetch data from cloud database
        Property lastInsertID As Integer = 0 'Get last id of inserted data to local database -> to not duplicate entry 
        Enum Stats
            isReady 'Default Sync Status
            isFetching 'Getting all available categories from cloud database
            isFetchInterrupt 'An error occured while fetching data
            isFetchComplete 'Fetch data completed
            isProcessing 'Inserting all the data from cloud to local database
            isProcessInterrupt 'An error occured while processing data
            isProcessComplete 'Whole process was complete
        End Enum
    End Class
End Class
