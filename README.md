# Validation of VAT or company registration numbers

* `VatNumber` structure
* `TryParse` (some numbers can match multiple countries and not return, try with countrycode first)
* `FormatStripped` for unique key in database (use together with CountryCode)
* `FormatNational` readable format of national number
* `FormatVat` readable format of VAT number
* `FormatVatStripped` stripped to minimal VAT representation for data (format not yet finall)

Countries still work in progress

<!-- COUNTRIES START -->
| Country | Prefix | Name | Short name | Length | Checksum | FormatNational | FormatStripped | FormatVat | FormatVatStripped | Status |
| ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| Austria | AT | Umsatzsteuer-Identifikationsnummer | UID-Nummer | 9 (U + 8) | luhn 10 +4 | ATU10223006 | ATU10223006 | ATU10223006 | ATU10223006 | Verified |
| Belgium | BE | BTW identificatienummer | BTW-nr | 10 (old 9) | mod 97 | 0566.988.259 | 0566988259 | BE 0566988259 | BE0566988259 | Verified |
| Bulgaria | BG | - | - | - | - | - | - | - | - | Not yet supported |
| Croatia | HR | - | - | - | - | - | - | - | - | Not yet supported |
| Cyprus | CY | - | - | - | - | - | - | - | - | Not yet supported |
| Czechia | CZ | - | - | - | - | - | - | - | - | Not yet supported |
| Denmark | DK | Momsregistreringsnummer | CVR | 8 | mod 11 | 25 31 37 63 | 25313763 | DK 25313763 | DK25313763 | Verified |
| Estonia | EE | Käibemaksukohustuslase number | KMKR | 9 | mod 10 | 100931558 | 100931558 | EE 100931558 | EE100931558 | Verified no official source |
| Finland | FI | Arvonlisäveronumero | ALV nro | 8 | mod 11-2 | 0201724-4 | 02017244 | FI 02017244 | FI02017244 | Verified official |
| France | FR | Numéro de TVA intracommunautaire / SIREN | N° TVA | 11, 9 | mod 97, luhn 10 | 404 833 048 | 404833048 | FR83 404 833 048 | FR83404833048 | Verified |
| Germany | DE | USt-IdNr | USt-IdNr | 8 | mod 11, mod 10, ISO 7064 | 115235681 | 115235681 | DE 115235681 | DE115235681 | Verified |
| Greece | EL | - | - | - | - | - | - | - | - | Not yet supported |
| Hungary | HU | - | - | - | - | - | - | - | - | Not yet supported |
| Ireland | IE | - | - | - | - | - | - | - | - | Not yet supported |
| Italy | IT | Imposta sul valore aggiunto | Partita IVA | 11 | luhn 10 | 07643520567 | 07643520567 | IT07643520567 | IT07643520567 | Verified |
| Latvia | LV | Pievienotās vērtības nodokļa reģistrācijas numurs | PVN | 11 | mod 11 | 40003521600 | 40003521600 | LV40003521600 | LV40003521600 | No official source |
| Lithuania | LT | Pridėtinės vertės mokestis mokėtojo kodas | PVM kodas | 9, 12 | mod 11 | 119511515 | 119511515 | LT119511515 | LT119511515 | No official source |
| Luxembourg | LU | Numéro d'identification à la taxe sur la valeur ajoutéee | No. TVA | 8 | mod 89 | 15027442 | 15027442 | LU 15027442 | LU15027442 | Verified |
| Malta | MT | - | - | - | - | - | - | - | - | Not yet supported |
| Netherlands | NL | Btw-nummer | Btw-nr. | 12 | mod 11 / mod 97 | NL004495445B01 | 004495445B01 | NL004495445B01 | NL004495445B01 | Verified |
| Norway | NO | Organisasjonsnummer | Orgnr | 9 | mod 11 | 977 074 010 | 977074010 | NO 977 074 010 | NO977074010 | Verified |
| Poland | PL | Numer identyfikacji podatkowej | NIP | 10 | mod 11 | 123-456-32-18 | 1234563218 | PL1234563218 | PL1234563218 | Verified |
| Portugal | PT | Número de Identificação de Pessoa Coletiva | NIPC | 9 | mod 11 | 999999990 | 999999990 | PT 999999990 | PT999999990 | Verified |
| Romania | RO | - | - | - | - | - | - | - | - | Not yet supported |
| Slovakia | SK | - | - | - | - | - | - | - | - | Not yet supported |
| Slovenia | SI | - | - | - | - | - | - | - | - | Not yet supported |
| Spain | ES | Número de Identificación Fiscal | NIF | 9 | luhn 10 | B12345674 | B12345674 | ES B12345674 | ESB12345674 | Verified no official |
| Sweden | SE | Momsnummer | Momsnr. | 12, 10 | luhn 10 | 101010-1010 | 1010101010 | SE 1010101010 01 | SE101010101001 | Verified official |
| Switzerland | CH | Numéro d'identification suisse des entreprises (IDE) | MWST/TVA/IVA | 10 | mod 11 | CHE-109.322.551 | CHE109322551 | CHE-109.322.551 | CHE109322551 | Verified |
<!-- COUNTRIES END -->