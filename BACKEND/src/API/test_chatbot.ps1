$url = "http://localhost:5000/api/Chat"
$body = @{
    message = "domates, süt, yumurta, ekmek için en ucuz market hangisi?"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType "application/json" -ErrorAction Stop
    
    $output = @()
    $output += "Response received successfully!"
    $output += "Reply: $($response.reply)"
    
    if ($response.basketSuggestion) {
        $output += "Cheapest Market: $($response.basketSuggestion.cheapestMarketName)"
        $output += "Total Price: $($response.basketSuggestion.totalPrice)"
        
        if ($response.basketSuggestion.missingItems.Count -gt 0) {
            $output += "Missing Items: $($response.basketSuggestion.missingItems -join ', ')"
        }
    }
    else {
        $output += "No basket suggestion received."
    }
    
    $output | Out-File -FilePath "test_output.txt" -Encoding utf8
    Write-Host "Output written to test_output.txt"
}
catch {
    $err = "Error: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $err += "`nDetails: $($reader.ReadToEnd())"
    }
    $err | Out-File -FilePath "test_output.txt" -Encoding utf8
    Write-Host "Error written to test_output.txt"
}
