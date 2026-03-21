$baseUrl = "http://localhost:5000/api"

Write-Host "Updating Portfolio Database with Latest Resume Data..." -ForegroundColor Green
Write-Host ""

# Add Work Experiences
Write-Host "Adding Work Experiences..." -ForegroundColor Cyan

$exp1 = @{
    title = "Tech Lead / Software Engineer III"
    company = "JP Morgan Chase & Co."
    startDate = "2023-08-01T00:00:00Z"
    endDate = $null
    description = "Service-to-Service (S2S) Authentication SDK: Spearheaded the design and implementation of an SDK for secure Authentication and Authorization between microservices, integrating multiple third-party identity providers to ensure robust service-level security. Atlas Cloud Migration: Led the strategic migration of business-critical applications from legacy AWS accounts to the Chase AWS ecosystem (Atlas cloud), ensuring 100% compliance with enterprise security and operational standards. Logs Standardization: Architect technical strategies for high-concurrency Travel domain applications leveraging AWS, .NET Core, and Angular. Citadel Portal: In-house Web Portal used by SRE team to provide product's health and compliance details. Roles & Responsibility: Feature Designing and Development, Troubleshooting issues, Improve product and deployment stability, Govern end-to-end CI/CD workflows via Jenkins, Collaborate with global stakeholders."
    technologies = @("C#", ".NET Core", "AWS", "Lambda", "S3", "CloudFront", "ECS", "CloudWatch", "Firehose", "Jenkins", "Docker", "CI/CD", "Angular", "Atlas Cloud")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp1 -UseBasicParsing | Out-Null
Write-Host "Added: Tech Lead / Software Engineer III at JP Morgan Chase" -ForegroundColor Green

$exp2 = @{
    title = "Senior Software Engineer"
    company = "Aumni Tech Works (Solve Inc.)"
    startDate = "2021-06-01T00:00:00Z"
    endDate = "2023-08-01T00:00:00Z"
    description = "Bond Market Product & Backoffice Admin Tool: Directed backend development and research for a US-based financial product specializing in the bond market. Resolved critical performance bottlenecks through technical research and optimization of core .NET Core services. Engineered an internal Admin Tool for complex user management and real-time visualization using .NET Core and ReactJS."
    technologies = @("C#", ".NET Core", "ReactJS", "REST APIs", "MSSQL", "Performance Optimization")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp2 -UseBasicParsing | Out-Null
Write-Host "Added: Senior Software Engineer at Aumni Tech Works" -ForegroundColor Green

$exp3 = @{
    title = "Senior Software Developer"
    company = "Tavisca Solutions Pvt. Ltd"
    startDate = "2014-02-01T00:00:00Z"
    endDate = "2021-06-01T00:00:00Z"
    description = "ORXe Micro-frontend Platform: Architected a multi-tenant platform using Angular and Lit components, enabling seamless client-wise customization for global travel brands. Flight & Hotel Booking Engines: Engineered global booking services integrating multiple supplier inventories and managing complex markup business logic for revenue generation. Infrastructure & Logging Optimization: Standardized logging fields across the product suite, leading to significant query performance improvements and infrastructure cost-efficiency. Identity Provider projects: Developed CRM Features (APEX portal) and Identity Management (VEXIERE) systems using .NET Core and Polymer 2.0."
    technologies = @("C#", ".NET Core", "ASP.NET MVC", "WCF", "Angular", "Lit Components", "Polymer 2.0", "Redis", "Elasticsearch", "ELK Stack", "Aerospike", "MSSQL", "AWS", "Docker")
} | ConvertTo-Json

Invoke-WebRequest -Uri "$baseUrl/experiences" -Method POST -ContentType "application/json" -Body $exp3 -UseBasicParsing | Out-Null
Write-Host "Added: Senior Software Developer at Tavisca Solutions" -ForegroundColor Green

Write-Host ""
Write-Host "Adding Skills..." -ForegroundColor Cyan

@(
    @{ name = "AWS"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Lambda"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "S3"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "CloudFront"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "ECS"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "CloudWatch"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Firehose"; category = "Cloud & DevOps"; proficiency = 4 },
    @{ name = "Jenkins"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Docker"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "CI/CD"; category = "Cloud & DevOps"; proficiency = 5 },
    @{ name = "Atlas Cloud"; category = "Cloud & DevOps"; proficiency = 4 },
    @{ name = "C#"; category = "Backend"; proficiency = 5 },
    @{ name = ".NET Core"; category = "Backend"; proficiency = 5 },
    @{ name = "ASP.NET MVC"; category = "Backend"; proficiency = 5 },
    @{ name = "WCF"; category = "Backend"; proficiency = 4 },
    @{ name = "Web Services"; category = "Backend"; proficiency = 5 },
    @{ name = "REST APIs"; category = "Backend"; proficiency = 5 },
    @{ name = "Node.js"; category = "Backend"; proficiency = 3 },
    @{ name = "MSSQL Server"; category = "Database"; proficiency = 5 },
    @{ name = "PostgreSQL"; category = "Database"; proficiency = 4 },
    @{ name = "Redis"; category = "Database"; proficiency = 5 },
    @{ name = "Elasticsearch"; category = "Database"; proficiency = 4 },
    @{ name = "ELK Stack"; category = "Database"; proficiency = 4 },
    @{ name = "Aerospike"; category = "Database"; proficiency = 3 },
    @{ name = "Couchbase"; category = "Database"; proficiency = 3 },
    @{ name = "Angular"; category = "Frontend"; proficiency = 5 },
    @{ name = "ReactJS"; category = "Frontend"; proficiency = 4 },
    @{ name = "TypeScript"; category = "Frontend"; proficiency = 5 },
    @{ name = "JavaScript"; category = "Frontend"; proficiency = 5 },
    @{ name = "Polymer 2.0"; category = "Frontend"; proficiency = 3 },
    @{ name = "HTML5"; category = "Frontend"; proficiency = 5 },
    @{ name = "CSS3"; category = "Frontend"; proficiency = 5 },
    @{ name = "Lit Components"; category = "Frontend"; proficiency = 4 }
) | ForEach-Object {
    $body = $_ | ConvertTo-Json
    Invoke-WebRequest -Uri "$baseUrl/skills" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing | Out-Null
    Write-Host "Added: $($_.name)" -ForegroundColor Green
}

Write-Host ""
Write-Host "Adding Certifications, Awards and Education..." -ForegroundColor Cyan

@(
    @{
        title = "AWS Certified Cloud Practitioner"
        description = "AWS Cloud Practitioner certification demonstrating cloud computing concepts and AWS services knowledge"
        technologies = @("AWS", "Cloud Computing", "Certification")
    },
    @{
        title = "The Pioneers Award (2017)"
        description = "Recognized for Outstanding Technical Impact at Tavisca Solutions"
        technologies = @("Award", "Recognition")
    },
    @{
        title = "The Rising Star Award (2016)"
        description = "Recognized for Exceptional Individual Contribution at Tavisca Solutions"
        technologies = @("Award", "Recognition")
    },
    @{
        title = "PG Diploma in Advanced Computing (PGDAC)"
        description = "Post Graduate Diploma in Advanced Computing from Vidyanidhi Info Tech Academy (CDAC), 2014"
        technologies = @("Education", "CDAC")
    },
    @{
        title = "Bachelor of Engineering (Computer Science)"
        description = "Bachelor of Engineering in Computer Science from Mumbai University, 2013"
        technologies = @("Education", "Mumbai University")
    }
) | ForEach-Object {
    $body = $_ | ConvertTo-Json
    Invoke-WebRequest -Uri "$baseUrl/projects" -Method POST -ContentType "application/json" -Body $body -UseBasicParsing | Out-Null
    Write-Host "Added: $($_.title)" -ForegroundColor Green
}

Write-Host ""
Write-Host "Resume data updated successfully!" -ForegroundColor Green
Write-Host "Refresh http://localhost:4200 to see your updated portfolio" -ForegroundColor Cyan
