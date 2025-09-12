# Validation of VAT or company registration numbers

* `VatNumber` structure
* `TryParse` (some numbers can match multiple countries and not return, try with countrycode first)
* `FormatStripped` for unique key in database (use together with CountryCode)
* `FormatNational` readable format of national number
* `FormatVat` readable format of VAT number
* `FormatVatStripped` stripped to minimal VAT representation for data (format not yet finall)

Countries still work in progress
