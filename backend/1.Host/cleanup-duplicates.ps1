$baseUrl = "http://localhost:5000/api"

Write-Host "Removing duplicate experiences..." -ForegroundColor Cyan
Write-Host ""

# Get all experiences
$allExp = Invoke-WebRequest -Uri "$baseUrl/experiences" -UseBasicParsing
$experiences = $allExp.Content | ConvertFrom-Json

# Normalize company names for comparison (remove trailing periods/spaces)
function CompanyName {
    param($name)
    return $name.Trim().TrimEnd('.')
}

# Group by normalized company and startDate to find duplicates
$groups = $experiences | Group-Object -Property @{Expression={(CompanyName $_.company) + "|" + $_.startDate}}

$deletedCount = 0
foreach ($group in $groups) {
    if ($group.Count -gt 1) {
        $companyName = $group.Group[0].company
        Write-Host "Found duplicates for: $companyName - $($group.Group[0].startDate)" -ForegroundColor Yellow
        
        # Sort by title length (keep the most detailed one) or by ID (keep newer)
        $sorted = $group.Group | Sort-Object -Property @{Expression={$_.title.Length}}, @{Expression={$_.id}} -Descending
        
        # Keep the first (most detailed/newest), delete the rest
        for ($i = 1; $i -lt $sorted.Count; $i++) {
            $idToDelete = $sorted[$i].id
            try {
                Invoke-WebRequest -Uri "$baseUrl/experiences/$idToDelete" -Method DELETE -UseBasicParsing
                Write-Host "  Deleted: $($sorted[$i].title) at $($sorted[$i].company) (ID: $idToDelete)" -ForegroundColor Green
                $deletedCount++
            }
            catch {
                Write-Host "  Error deleting $idToDelete : $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }
}

Write-Host ""
Write-Host "Cleanup complete! Deleted $deletedCount duplicate experience(s)." -ForegroundColor Green
Write-Host "Refresh your browser to see the updated list." -ForegroundColor Cyan
