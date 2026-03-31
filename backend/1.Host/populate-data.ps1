$baseUrl = "http://localhost:5000/api"

Write-Host "Adding Work Experiences..." -ForegroundColor Cyan

$exp1 = @{
    title = "Software Engineer III"
    company = "JP Morgan Chase & Co."
    startDate = "2023-08-01T00:00:00Z"
    endDate = $null
    description = "Service 2 Service Auth Initiative: Drove service-to-service integration and standardization across backend systems. Built internal logging field standardization app improving observability. Delivered Citadel UI Portal for health/compliance visibility. Mentored engineers and collaborated with architects for timely delivery."
    technologies = @("C#", ".NET Core", "AWS", "Lambda", "ECS", "Redis", "Jenkins", "Git")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp1 -UseBasicParsing | Out-Null
Write-Host "Added: Software Engineer III at JP Morgan Chase" -ForegroundColor Green

$exp2 = @{
    title = "Senior Software Engineer"
    company = "Aumni Tech Works (Solve Inc.)"
    startDate = "2021-06-01T00:00:00Z"
    endDate = "2023-08-01T00:00:00Z"
    description = "Backend ownership for US bond-market product with cross-functional US stakeholder collaboration. Built Admin Tool for user management, configuration, data visualizations, and filtration rules. Contributed to ReactJS features and production support."
    technologies = @("C#", ".NET Core", "ReactJS", "REST APIs", "AWS")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp2 -UseBasicParsing | Out-Null
Write-Host "Added: Senior Software Engineer at Aumni Tech Works" -ForegroundColor Green

$exp3 = @{
    title = "Senior Software Developer"
    company = "Tavisca Solutions Pvt. Ltd."
    startDate = "2014-02-01T00:00:00Z"
    endDate = "2021-06-01T00:00:00Z"
    description = "Core product development across travel platforms; delivered features, web services, and production support. ORXe Micro Frontend Platform enabling client-specific customization across Angular/Lit components. Engines/services: Booking, Payment, Markup, Flight/Hotel/Car/Activity with multi-supplier integrations, caching, and ELK observability."
    technologies = @("C#", ".NET Core", "MVC", "WCF", "REST", "Redis", "Elasticsearch", "Aerospike", "MSSQL", "AWS", "Docker", "Angular")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp3 -UseBasicParsing | Out-Null
Write-Host "Added: Senior Software Developer at Tavisca Solutions" -ForegroundColor Green

Write-Host "`nAdding Skills..." -ForegroundColor Cyan

@(
    @{ name = "C#"; category = "Backend"; proficiency = 5 },
    @{ name = ".NET Core"; category = "Backend"; proficiency = 5 },
    @{ name = "ASP.NET MVC"; category = "Backend"; proficiency = 5 },
    @{ name = "AWS"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Lambda"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Docker"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Jenkins"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "MSSQL"; category = "Data"; proficiency = 5 },
    @{ name = "PostgreSQL"; category = "Data"; proficiency = 5 },
    @{ name = "Redis"; category = "Data"; proficiency = 5 },
    @{ name = "Angular"; category = "Frontend"; proficiency = 4 },
    @{ name = "TypeScript"; category = "Frontend"; proficiency = 5 },
    @{ name = "React"; category = "Frontend"; proficiency = 4 }
) | ForEach-Object {
    $body = $_ | ConvertTo-Json
    Invoke-WebRequest -Uri "$baseUrl/skills" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing | Out-Null
    Write-Host "Added: $($_.name)" -ForegroundColor Green
}

Write-Host "`nResume data added successfully!" -ForegroundColor Green
Write-Host "Refresh http://localhost:4200 to see your portfolio" -ForegroundColor Cyan

# Add Projects (Awards & Education)
Write-Host "`nAdding Awards & Education..." -ForegroundColor Cyan

@(
    @{
        title = "Rising Star Award"
        description = "Recognition for outstanding performance and contributions at Tavisca Solutions (2016)"
        technologies = @("Award")
        featured = $true
    },
    @{
        title = "The Pioneers Award"
        description = "Recognition for pioneering work and innovation at Tavisca Solutions (2017)"
        technologies = @("Award")
        featured = $true
    },
    @{
        title = "PGDAC - CDAC Certification"
        description = "Post Graduate Diploma in Advanced Computing from CDAC, Vidyanidhi Info Tech Academy (2014)"
        technologies = @("Education")
        featured = $true
    },
    @{
        title = "B.E. Computer Engineering"
        description = "Bachelor of Engineering in Computer Engineering from S.S. Jondhale C.E.T. (2013)"
        technologies = @("Education")
        featured = $false
    },
    @{
        title = "Diploma in Computer Technology"
        description = "Diploma in Computer Technology from S.S. Jondhle Polytechnic (2010)"
        technologies = @("Education")
        featured = $false
    }
) | ForEach-Object {
    $body = $_ | ConvertTo-Json
    Invoke-WebRequest -Uri "$baseUrl/projects" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing | Out-Null
    Write-Host "Added: $($_.title)" -ForegroundColor Green
}
