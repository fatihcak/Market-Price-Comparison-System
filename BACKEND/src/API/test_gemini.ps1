try {
    $apiKey = "AIzaSyAdmjufCj03Jw-FV3GIKv90yYHrev491XQ"
    $url = "https://generativelanguage.googleapis.com/v1beta/models?key=$apiKey"
    
    $response = Invoke-WebRequest -Method Get -Uri $url -ErrorAction Stop
    $response.Content | Out-File -Encoding utf8 gemini_response.txt
}
catch {
    $_.Exception.Response.GetResponseStream() | % { 
        $reader = New-Object System.IO.StreamReader($_)
        $reader.ReadToEnd() | Out-File -Encoding utf8 gemini_response.txt
    }
}
