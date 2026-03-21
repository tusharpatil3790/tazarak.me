# Add Resume Data to Portfolio Database
# This script populates the database with resume data (experiences, skills, projects)

$baseUrl = "http://localhost:5000/api"

Write-Host "Adding Resume Data to Portfolio Database..." -ForegroundColor Green
Write-Host "Make sure the API is running at $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Add Work Experiences
$experiences = @(
    @{
        title = "Software Engineer III"
        company = "JP Morgan Chase & Co."
        startDate = "2023-08-01T00:00:00Z"
        endDate = $null
        description = "Service 2 Service Auth Initiative: Drove service-to-service integration and standardization across backend systems. Built internal logging field standardization app improving observability. Delivered Citadel UI Portal for health/compliance visibility. Mentored engineers and collaborated with architects for timely delivery. Stack: .NET Core, AWS (Lambda, ECS, CloudWatch), Redis, Jenkins, Git"
        technologies = @("C#", ".NET Core", "AWS", "Lambda", "ECS", "Redis", "Jenkins", "Git", "Terraform")
    },
    @{
        title = "Senior Software Engineer"
        company = "Aumni Tech Works (Solve Inc.)"
        startDate = "2021-06-01T00:00:00Z"
        endDate = "2023-08-01T00:00:00Z"
        description = "Backend ownership for US bond-market product with cross-functional US stakeholder collaboration. Built Admin Tool for user management, configuration, data visualizations, and filtration rules. Contributed to ReactJS features and production support. Improved operational efficiency across platform."
        technologies = @("C#", ".NET Core", "ReactJS", "REST APIs", "Cloud Services")
    }
)

# Add Skills (proficiency: 1-5, where 5 is Expert)
$skills = @(
    @{ name = "C#"; category = "Backend"; proficiency = 5 },
    @{ name = ".NET Core"; category = "Backend"; proficiency = 5 },
    @{ name = "ASP.NET MVC"; category = "Backend"; proficiency = 5 },
    @{ name = "REST APIs"; category = "Backend"; proficiency = 5 },
    @{ name = "WCF"; category = "Backend"; proficiency = 3 },
    @{ name = "AWS"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Lambda"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "ECS"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "S3"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "CloudFront"; category = "Cloud & DevOps"; proficiency = 3 },
    @{ name = "CloudWatch"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Jenkins"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Docker"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Git"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "CI/CD"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Terraform"; category = "Cloud & DevOps"; proficiency = 3 },
    @{ name = "MSSQL"; category = "Data"; proficiency = 5 },
    @{ name = "PostgreSQL"; category = "Data"; proficiency = 5 },
    @{ name = "Redis"; category = "Data"; proficiency = 5 },
    @{ name = "Elasticsearch"; category = "Data"; proficiency = 3 },
    @{ name = "Aerospike"; category = "Data"; proficiency = 3 },
    @{ name = "Angular"; category = "Frontend"; proficiency = 4 },
    @{ name = "TypeScript"; category = "Frontend"; proficiency = 5 },
    @{ name = "React"; category = "Frontend"; proficiency = 4 },
    @{ name = "JavaScript"; category = "Frontend"; proficiency = 5 },
    @{ name = "HTML"; category = "Frontend"; proficiency = 5 },
    @{ name = "CSS"; category = "Frontend"; proficiency = 5 }
)

# Add Certifications as Projects
$certifications = @(
    @{
        title = "AWS Certified Cloud Practitioner"
        description = "AWS Cloud Practitioner certification demonstrating cloud computing concepts and AWS services knowledge"
        technologies = @("AWS", "Cloud Computing")
        gitHubUrl = $null
        liveUrl = $null
        imageUrl = $null
    }
)

Write-Host "Adding Work Experiences..." -ForegroundColor Cyan
foreach ($exp in $experiences) {
    $body = $exp | ConvertTo-Json
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing
        Write-Host "  ✓ Added: $($exp.title) at $($exp.company)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Error adding experience: $($exp.title)" -ForegroundColor Red
        Write-Host "    $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nAdding Skills..." -ForegroundColor Cyan
foreach ($skill in $skills) {
    $body = $skill | ConvertTo-Json
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/skills" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing
        Write-Host "  ✓ Added: $($skill.name) ($($skill.category))" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Error adding skill: $($skill.name)" -ForegroundColor Red
        Write-Host "    $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nAdding Certifications..." -ForegroundColor Cyan
foreach ($cert in $certifications) {
    $body = $cert | ConvertTo-Json
    try {
        $response = Invoke-WebRequest -Uri "$baseUrl/projects" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing
        Write-Host "  ✓ Added: $($cert.title)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ✗ Error adding certification: $($cert.title)" -ForegroundColor Red
        Write-Host "    $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n✓ Resume data added successfully!" -ForegroundColor Green
Write-Host "Visit http://localhost:4200 to see your portfolio" -ForegroundColor Cyan
