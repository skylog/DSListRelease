#############################################################################
# AD-users.ps1
# Список активных пользователей AD
#############################################################################
Import-Module ActiveDirectory
$filesreport = "AD-enabled-users.html"
##HTML##
$a = "<style>"
$a = $a + "BODY{background-color:#ffffff;font-family: Arial; font-size: 8pt;}"
$a = $a + "TABLE{border-width: 1px;border-style: solid;border-color: black;border-collapse: collapse;}"
$a = $a + "TH{border-width: 1px;padding: 20px;border-style: solid;border-color: black;background-color:#dddddd}"
$a = $a + "TD{border-width: 1px;padding: 10px;border-style: solid;border-color: black;background-color:#ffffff}"
$a = $a + "</style>"

##Get users
Get-ADUser -Filter {enabled -ne $false} -Properties "whenCreated","SamAccountName","mail","mobile","telephoneNumber","sn","givenName","middleName","department","title"|
# Sorting. New on top
sort whenCreated -Descending | 
Select-Object -Property "whenCreated","SamAccountName","mail","mobile","telephoneNumber","sn","givenName","middleName","department","title"|
# Convert to HTML
ConvertTo-HTML -head $a|Out-File $filesreport

##Show message box##
##$MsgTitle = "Список активных пользователей Active Directory"
##$MsgText = "Создан файл отчёта AD-enabled-users.html`nОткрыть файл в браузере?"
##$WShell = New-Object -ComObject Wscript.Shell
##$OKCancel = $WShell.Popup($MsgText,0,$MsgTitle,0x1)

##switch($OKCancel){
##1{
##Open HTML in default browser##
##Start('AD-enabled-users.html')
##}
##2{exit}
##}