const API_URL = 'https://localhost:5001/api';

// Check API health
async function checkApiHealth() {
    try {
        const response = await fetch(`${API_URL}/experiences`);
        if (response.ok) {
            document.getElementById('apiStatus').textContent = '✓ API Connected';
            document.getElementById('apiStatus').classList.remove('offline');
            loadData();
        } else {
            throw new Error('API returned non-OK status');
        }
    } catch (error) {
        console.error('API Health Check Failed:', error);
        document.getElementById('apiStatus').textContent = '✗ API Offline';
        document.getElementById('apiStatus').classList.add('offline');
    }
}

// Load experiences
async function loadExperiences() {
    try {
        console.log('Fetching experiences from:', `${API_URL}/experiences`);
        const response = await fetch(`${API_URL}/experiences`);
        if (!response.ok) {
            console.error('Experiences response not OK:', response.status, response.statusText);
            throw new Error(`HTTP ${response.status}`);
        }
        const experiences = await response.json();
        console.log('Loaded experiences:', experiences);
        const container = document.getElementById('experiences');
        if (!experiences || experiences.length === 0) {
            container.innerHTML = '<div class="experience-item"><p>No experiences found. Add some via the API.</p></div>';
        } else {
            container.innerHTML = experiences.map(exp => `
                <div class="experience-item">
                    <h3>${exp.title || 'Untitled'}</h3>
                    <p><strong>${exp.company || 'Company'}</strong> | ${exp.startDate || 'N/A'} - ${exp.endDate || 'Present'}</p>
                    <p>${exp.description || ''}</p>
                </div>
            `).join('');
        }
    } catch (error) {
        console.error('Error loading experiences:', error);
        document.getElementById('experiences').innerHTML = `<div class="experience-item"><p style="color: red;">Failed to load experiences: ${error.message}</p></div>`;
    }
}

// Load skills
async function loadSkills() {
    try {
        const response = await fetch(`${API_URL}/skills`);
        const skills = await response.json();
        const container = document.getElementById('skillsList');
        container.innerHTML = skills.map(skill => `
            <div class="skill-item">
                <h3>${skill.name || 'Skill'}</h3>
                <p>Proficiency: ${skill.proficiency || 'N/A'}</p>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading skills:', error);
    }
}

// Load projects
async function loadProjects() {
    try {
        const response = await fetch(`${API_URL}/projects`);
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }
        const projects = await response.json();
        const container = document.getElementById('projectsList');
        
        if (!projects || projects.length === 0) {
            container.innerHTML = '<div class="project-item"><p>No projects found. Add some via the API.</p></div>';
            return;
        }
        
        // Categorize projects
        const certifications = projects.filter(p => {
            const title = (p.title || '').toLowerCase();
            const techs = (p.technologies || []).map(t => t.toLowerCase()).join(' ');
            return title.includes('certified') || title.includes('certification') ||
                   techs.includes('certification') || techs.includes('aws certified');
        });
        const awards = projects.filter(p => {
            const title = (p.title || '').toLowerCase();
            const techs = (p.technologies || []).map(t => t.toLowerCase()).join(' ');
            return title.includes('award') || techs.includes('award') || techs.includes('recognition');
        });
        const education = projects.filter(p => {
            const title = (p.title || '').toLowerCase();
            const techs = (p.technologies || []).map(t => t.toLowerCase()).join(' ');
            return title.includes('diploma') || title.includes('degree') || title.includes('engineering') ||
                   title.includes('bachelor') || title.includes('pgdac') ||
                   techs.includes('education') || techs.includes('cdac') || techs.includes('university');
        });
        
        function renderProject(project) {
            const techTags = project.technologies && project.technologies.length > 0
                ? project.technologies.map(tech => `<span class="tech-tag">${tech}</span>`).join('')
                : '';
            const links = [];
            if (project.gitHubUrl) {
                links.push(`<a href="${project.gitHubUrl}" target="_blank">🔗 GitHub</a>`);
            }
            if (project.liveUrl) {
                links.push(`<a href="${project.liveUrl}" target="_blank">🌐 Live Demo</a>`);
            }
            
            return `
                <div class="project-item">
                    <h3>${project.title || 'Project'}</h3>
                    <p>${project.description || ''}</p>
                    ${techTags ? `<div class="tech-tags">${techTags}</div>` : ''}
                    ${links.length > 0 ? `<p style="margin-top: 0.5rem;">${links.join(' ')}</p>` : ''}
                </div>
            `;
        }
        
        let html = '';
        
        if (certifications.length > 0) {
            html += '<div class="project-category"><h3>Certifications</h3>';
            html += certifications.map(renderProject).join('');
            html += '</div>';
        }
        
        if (awards.length > 0) {
            html += '<div class="project-category"><h3>Awards & Recognition</h3>';
            html += awards.map(renderProject).join('');
            html += '</div>';
        }
        
        if (education.length > 0) {
            html += '<div class="project-category"><h3>Education</h3>';
            html += education.map(renderProject).join('');
            html += '</div>';
        }
        
        // Add any uncategorized projects
        const categorized = [...certifications, ...awards, ...education];
        const uncategorized = projects.filter(p => !categorized.includes(p));
        if (uncategorized.length > 0) {
            html += '<div class="project-category"><h3>Other</h3>';
            html += uncategorized.map(renderProject).join('');
            html += '</div>';
        }
        
        container.innerHTML = html || '<div class="project-item"><p>No projects found.</p></div>';
    } catch (error) {
        console.error('Error loading projects:', error);
        document.getElementById('projectsList').innerHTML = `<div class="project-item"><p style="color: red;">Failed to load projects: ${error.message}</p></div>`;
    }
}

// Load all data
function loadData() {
    loadExperiences();
    loadSkills();
    loadProjects();
}

// Handle contact form submission
document.getElementById('contactForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const [name, email, phone, subject, message] = [
        formData.get('name') || e.target[0].value,
        formData.get('email') || e.target[1].value,
        e.target[2].value,
        e.target[3].value,
        e.target[4].value
    ];
    
    try {
        const response = await fetch(`${API_URL}/contacts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, email, phone, subject, message })
        });
        if (response.ok) {
            alert('Message sent successfully!');
            e.target.reset();
        } else {
            alert('Failed to send message');
        }
    } catch (error) {
        console.error('Error sending message:', error);
        alert('Error sending message');
    }
});

// Check API on page load
checkApiHealth();
setInterval(checkApiHealth, 30000); // Check every 30 seconds
