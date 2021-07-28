$client_id = $env:CLIENT_ID
$client_secret = $env:CLIENT_SECRET
if ([System.String]::IsNullOrEmpty($client_id)) {
    $client_id = Read-Host -Prompt "Enter Forge Client Id"
    $env:CLIENT_ID = $client_id
}

if ([System.String]::IsNullOrEmpty($client_secret)) {
    $client_secret = Read-Host -Prompt "Enter Forge Client Secret"
    $env:CLIENT_SECRET = $client_secret
}
$headers = @{}
$headers.Add("Content-Type", "application/x-www-form-urlencoded")
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$cookie = New-Object System.Net.Cookie
$cookie.Name = 'PF'
$cookie.Value = '3UPz62jMwpz6oZp4APHxp6'
$cookie.Domain = 'developer.api.autodesk.com'
$session.Cookies.Add($cookie)
$response = Invoke-WebRequest -Uri "https://developer.api.autodesk.com/authentication/v1/authenticate" -Method POST -Headers $headers -WebSession $session -ContentType "application/x-www-form-urlencoded" -Body "client_id=${client_id}&scope=data%3Awrite%20data%3Aread%20bucket%3Aread%20bucket%3Aupdate%20bucket%3Acreate%20code%3Aall&grant_type=client_credentials&client_secret=${client_secret}&="
$response = ConvertFrom-Json -InputObject $response.Content
$bearer = "Bearer $($response.access_token)"
$headers = @{}
$headers.Add("Content-Type", "application/json")
$headers.Add("Authorization", $bearer)
$publicKey = Get-Content ".\mypublickey.json"
$body = "{'publicKey':$publicKey}"
$response = Invoke-WebRequest -Uri 'https://developer.api.autodesk.com/da/us-east/v3/forgeapps/me' -Method PATCH -Headers $headers -WebSession $session -ContentType 'application/json' -Body $body
Write-Output $response