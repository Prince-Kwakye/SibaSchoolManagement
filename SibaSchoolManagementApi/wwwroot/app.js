document.addEventListener('DOMContentLoaded', () => {
    // Configuration
    const API_BASE_URL = 'https://localhost:7087/api';
    const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

    // State management
    const state = {
        currentUser: null,
        students: [],
        courses: [],
        timetable: [],
        authToken: null
    };

    // DOM elements
    const mainContent = document.getElementById('main-content');
    const mainNav = document.getElementById('main-nav');
    const userInfo = document.getElementById('user-info');

    // Initialize the application
    init();

    function init() {
        // Check for authenticated user in sessionStorage
        const user = sessionStorage.getItem('currentUser');
        const token = sessionStorage.getItem('authToken');
        
        if (user && token) {
            state.currentUser = JSON.parse(user);
            state.authToken = token;
            renderAuthenticatedUI();
        } else {
            renderLoginUI();
        }
    }

    // API Helper Functions
    async function fetchWithAuth(url, options = {}) {
        const headers = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${state.authToken}`
        };

        const response = await fetch(`${API_BASE_URL}${url}`, {
            ...options,
            headers: {
                ...headers,
                ...(options.headers || {})
            }
        });

        if (response.status === 401) {
            // Token expired or invalid
            handleLogout();
            return;
        }

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.message || 'Request failed');
        }

        return response.json();
    }

    // UI Rendering Functions
    function renderLoginUI() {
        mainContent.innerHTML = `
            <section id="login-section" class="active">
                <h2>Administration Login</h2>
                <form id="login-form">
                    <div class="form-group">
                        <label for="username">Username</label>
                        <input type="text" id="username" required>
                    </div>
                    <div class="form-group">
                        <label for="password">Password</label>
                        <input type="password" id="password" required>
                    </div>
                    <button type="submit" class="btn-primary">Login</button>
                </form>
            </section>
        `;
        
        document.getElementById('login-form').addEventListener('submit', handleLogin);
    }

    function renderAuthenticatedUI() {
        // Update user info in header
        userInfo.innerHTML = `
            <span>Welcome, ${state.currentUser.username}</span>
            <button id="logout-btn" class="btn">Logout</button>
        `;
        
        document.getElementById('logout-btn').addEventListener('click', handleLogout);
        
        // Update navigation
        mainNav.innerHTML = `
            <a href="#" data-section="dashboard">Dashboard</a>
            <a href="#" data-section="students">Students</a>
            <a href="#" data-section="courses">Courses</a>
            <a href="#" data-section="timetable">Timetable</a>
        `;
        
        // Add navigation event listeners
        document.querySelectorAll('#main-nav a').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                loadSection(link.dataset.section);
            });
        });
        
        // Load default section
        loadSection('dashboard');
    }

    async function loadSection(section) {
        // Hide all sections
        document.querySelectorAll('#main-content section').forEach(sec => {
            sec.classList.remove('active');
        });
        
        // Show the selected section
        let sectionElement = document.getElementById(`${section}-section`);
        
        if (!sectionElement) {
            // Create the section if it doesn't exist
            sectionElement = document.createElement('section');
            sectionElement.id = `${section}-section`;
            mainContent.appendChild(sectionElement);
            
            // Load section content based on the section name
            switch(section) {
                case 'dashboard':
                    await loadDashboard(sectionElement);
                    break;
                case 'students':
                    await loadStudentsSection(sectionElement);
                    break;
                case 'courses':
                    await loadCoursesSection(sectionElement);
                    break;
                case 'timetable':
                    await loadTimetableSection(sectionElement);
                    break;
            }
        }
        
        sectionElement.classList.add('active');
    }

    async function loadDashboard(sectionElement) {
        sectionElement.innerHTML = `
            <h2>Dashboard</h2>
            <div class="stats-container">
                <div class="stat-card">
                    <h3>Total Students</h3>
                    <p id="total-students">Loading...</p>
                </div>
                <div class="stat-card">
                    <h3>Total Courses</h3>
                    <p id="total-courses">Loading...</p>
                </div>
                <div class="stat-card">
                    <h3>Active Registrations</h3>
                    <p id="active-registrations">Loading...</p>
                </div>
            </div>
            <div class="recent-activity">
                <h3>Recent Activity</h3>
                <ul id="activity-list"></ul>
            </div>
        `;
        
        try {
            // Fetch data from backend
            const [students, courses] = await Promise.all([
                fetchWithAuth('/Students'),
                fetchWithAuth('/Courses')
            ]);
            
            // Update stats
            document.getElementById('total-students').textContent = students.length;
            document.getElementById('total-courses').textContent = courses.length;
            
           
            document.getElementById('active-registrations').textContent = 
                Math.floor(students.length * 0.7); 
            
            
            const activityList = document.getElementById('activity-list');
            const activities = [
                'System initialized successfully',
                `${students.length} students loaded`,
                `${courses.length} courses loaded`,
                'Ready for administration'
            ];
            
            activities.forEach(activity => {
                const li = document.createElement('li');
                li.textContent = activity;
                activityList.appendChild(li);
            });
        } catch (error) {
            console.error('Dashboard loading error:', error);
            alert('Failed to load dashboard data');
        }
    }

    async function loadStudentsSection(sectionElement) {
        sectionElement.innerHTML = `
            <h2>Student Management</h2>
            <div class="action-bar">
                <button id="add-student-btn" class="btn-primary">Add New Student</button>
                <div class="search-box">
                    <input type="text" id="student-search" placeholder="Search students...">
                    <button id="search-student-btn" class="btn">Search</button>
                </div>
            </div>
            <div class="table-container">
                <table id="students-table">
                    <thead>
                        <tr>
                            <th>Student ID</th>
                            <th>Name</th>
                            <th>Date of Birth</th>
                            <th>Gender</th>
                            <th>Registration Date</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody id="students-table-body">
                        <!-- Students will be loaded here -->
                    </tbody>
                </table>
            </div>
            <div id="student-form-modal" class="modal">
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3 id="student-form-title">Add New Student</h3>
                    <form id="student-form">
                        <input type="hidden" id="student-id">
                        <div class="form-group">
                            <label for="student-id-input">Student ID</label>
                            <input type="text" id="student-id-input" required>
                        </div>
                        <div class="form-group">
                            <label for="first-name">First Name</label>
                            <input type="text" id="first-name" required>
                        </div>
                        <div class="form-group">
                            <label for="last-name">Last Name</label>
                            <input type="text" id="last-name" required>
                        </div>
                        <div class="form-group">
                            <label for="dob">Date of Birth</label>
                            <input type="date" id="dob" required>
                        </div>
                        <div class="form-group">
                            <label for="gender">Gender</label>
                            <select id="gender" required>
                                <option value="">Select Gender</option>
                                <option value="Male">Male</option>
                                <option value="Female">Female</option>
                                <option value="Other">Other</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label for="email">Email</label>
                            <input type="email" id="email">
                        </div>
                        <div class="form-group">
                            <label for="phone">Phone</label>
                            <input type="tel" id="phone">
                        </div>
                        <div class="form-group">
                            <label for="address">Address</label>
                            <textarea id="address" rows="3"></textarea>
                        </div>
                        <button type="submit" class="btn-primary">Save</button>
                    </form>
                </div>
            </div>
            <div id="course-assignment-modal" class="modal">
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3>Assign Courses to Student</h3>
                    <div id="available-courses-list"></div>
                    <button id="save-courses-btn" class="btn-primary">Save Assignments</button>
                </div>
            </div>
        `;
        
        // Load student data
        await loadStudentData();
        
        // Add event listeners
        document.getElementById('add-student-btn').addEventListener('click', () => {
            openStudentForm();
        });
        
        document.getElementById('student-form').addEventListener('submit', handleStudentFormSubmit);
        
        document.querySelectorAll('.close-modal').forEach(btn => {
            btn.addEventListener('click', () => {
                document.getElementById('student-form-modal').style.display = 'none';
                document.getElementById('course-assignment-modal').style.display = 'none';
            });
        });
        
        document.getElementById('search-student-btn').addEventListener('click', async (e) => {
            e.preventDefault();
            const searchTerm = document.getElementById('student-search').value.toLowerCase();
            await loadStudentData(searchTerm);
        });
    }

    async function loadStudentData(searchTerm = '') {
        try {
            let url = '/Students';
            if (searchTerm) {
                url += `?search=${encodeURIComponent(searchTerm)}`;
            }
            
            state.students = await fetchWithAuth(url);
            renderStudentTable();
        } catch (error) {
            console.error('Error loading students:', error);
            alert('Failed to load student data');
        }
    }

    function renderStudentTable() {
        const tbody = document.getElementById('students-table-body');
        tbody.innerHTML = '';
        
        state.students.forEach(student => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${student.studentId}</td>
                <td>${student.firstName} ${student.lastName}</td>
                <td>${formatDate(student.dateOfBirth)}</td>
                <td>${student.gender}</td>
                <td>${formatDate(student.registrationDate)}</td>
                <td>${student.isActive ? 'Active' : 'Inactive'}</td>
                <td>
                    <button class="btn edit-student" data-id="${student.id}">Edit</button>
                    <button class="btn assign-courses" data-id="${student.id}">Assign Courses</button>
                    <button class="btn view-timetable" data-id="${student.id}">View Timetable</button>
                </td>
            `;
            
            tbody.appendChild(tr);
        });
        
        // Add event listeners to buttons
        document.querySelectorAll('.edit-student').forEach(btn => {
            btn.addEventListener('click', () => {
                const studentId = parseInt(btn.dataset.id);
                editStudent(studentId);
            });
        });
        
        document.querySelectorAll('.assign-courses').forEach(btn => {
            btn.addEventListener('click', async () => {
                const studentId = parseInt(btn.dataset.id);
                await assignCoursesToStudent(studentId);
            });
        });
        
        document.querySelectorAll('.view-timetable').forEach(btn => {
            btn.addEventListener('click', async () => {
                const studentId = parseInt(btn.dataset.id);
                await viewStudentTimetable(studentId);
            });
        });
    }

    function openStudentForm(student = null) {
        const modal = document.getElementById('student-form-modal');
        const form = document.getElementById('student-form');
        const title = document.getElementById('student-form-title');
        
        if (student) {
            title.textContent = 'Edit Student';
            document.getElementById('student-id').value = student.id;
            document.getElementById('student-id-input').value = student.studentId;
            document.getElementById('first-name').value = student.firstName;
            document.getElementById('last-name').value = student.lastName;
            document.getElementById('dob').value = student.dateOfBirth.split('T')[0];
            document.getElementById('gender').value = student.gender;
            document.getElementById('email').value = student.email || '';
            document.getElementById('phone').value = student.phone || '';
            document.getElementById('address').value = student.address || '';
        } else {
            title.textContent = 'Add New Student';
            form.reset();
        }
        
        modal.style.display = 'block';
    }

    async function editStudent(studentId) {
        try {
            const student = await fetchWithAuth(`/Students/${studentId}`);
            if (student) {
                openStudentForm(student);
            }
        } catch (error) {
            console.error('Error editing student:', error);
            alert('Failed to load student data');
        }
    }

    async function assignCoursesToStudent(studentId) {
        try {
            // Get all courses and student's enrolled courses
            const [allCourses, studentCourses] = await Promise.all([
                fetchWithAuth('/Courses'),
                fetchWithAuth(`/Students/${studentId}/courses`)
            ]);
            
            const modal = document.getElementById('course-assignment-modal');
            const coursesList = document.getElementById('available-courses-list');
            
            coursesList.innerHTML = '';
            
            // Create checkboxes for each course
            allCourses.forEach(course => {
                const isEnrolled = studentCourses.some(sc => sc.id === course.id);
                
                const div = document.createElement('div');
                div.className = 'course-checkbox';
                div.innerHTML = `
                    <input type="checkbox" id="course-${course.id}" value="${course.id}" 
                        ${isEnrolled ? 'checked' : ''}>
                    <label for="course-${course.id}">${course.code} - ${course.name}</label>
                `;
                coursesList.appendChild(div);
            });
            
            // Set up save button
            const saveBtn = document.getElementById('save-courses-btn');
            saveBtn.onclick = async () => {
                const selectedCourses = Array.from(
                    document.querySelectorAll('#available-courses-list input[type="checkbox"]:checked')
                ).map(cb => parseInt(cb.value));
                
                try {
                    await fetchWithAuth(`/Students/${studentId}/courses`, {
                        method: 'POST',
                        body: JSON.stringify(selectedCourses)
                    });
                    
                    modal.style.display = 'none';
                    alert('Courses assigned successfully');
                } catch (error) {
                    console.error('Error assigning courses:', error);
                    alert('Failed to assign courses');
                }
            };
            
            modal.style.display = 'block';
        } catch (error) {
            console.error('Error loading courses:', error);
            alert('Failed to load course data');
        }
    }

    async function viewStudentTimetable(studentId) {
        try {
            // Get student's courses
            const courses = await fetchWithAuth(`/Students/${studentId}/courses`);
            
            // Get timetable for these courses
            const timetable = await fetchWithAuth('/Timetable');
            
            // Filter timetable for student's courses
            const studentTimetable = timetable.filter(slot => 
                courses.some(course => course.id === slot.courseId)
            );
            
            // Open timetable in a new window for printing
            const printWindow = window.open('', '_blank');
            printWindow.document.write(`
                <html>
                    <head>
                        <title>Student Timetable</title>
                        <style>
                            body { font-family: Arial, sans-serif; }
                            h1 { text-align: center; }
                            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                            th, td { border: 1px solid #000; padding: 8px; text-align: left; }
                            th { background-color: #f2f2f2; }
                        </style>
                    </head>
                    <body>
                        <h1>Student Timetable</h1>
                        <table>
                            <thead>
                                <tr>
                                    <th>Course</th>
                                    <th>Day</th>
                                    <th>Time</th>
                                    <th>Room</th>
                                </tr>
                            </thead>
                            <tbody>
                                ${studentTimetable.map(slot => {
                                    const course = courses.find(c => c.id === slot.courseId);
                                    return `
                                        <tr>
                                            <td>${course ? course.name : 'Unknown'}</td>
                                            <td>${DAY_NAMES[slot.dayOfWeek]}</td>
                                            <td>${slot.startTime} - ${slot.endTime}</td>
                                            <td>${slot.roomNumber || ''}</td>
                                        </tr>
                                    `;
                                }).join('')}
                            </tbody>
                        </table>
                        <script>
                            window.onload = function() {
                                window.print();
                                setTimeout(function() {
                                    window.close();
                                }, 1000);
                            };
                        </script>
                    </body>
                </html>
            `);
            printWindow.document.close();
        } catch (error) {
            console.error('Error viewing timetable:', error);
            alert('Failed to load timetable data');
        }
    }

    async function handleStudentFormSubmit(e) {
        e.preventDefault();
        
        const studentId = document.getElementById('student-id').value;
        const studentData = {
            studentId: document.getElementById('student-id-input').value,
            firstName: document.getElementById('first-name').value,
            lastName: document.getElementById('last-name').value,
            dateOfBirth: document.getElementById('dob').value,
            gender: document.getElementById('gender').value,
            email: document.getElementById('email').value,
            phone: document.getElementById('phone').value,
            address: document.getElementById('address').value
        };
        
        try {
            if (studentId) {
                // Update existing student
                await fetchWithAuth(`/Students/${studentId}`, {
                    method: 'PUT',
                    body: JSON.stringify({
                        ...studentData,
                        isActive: true
                    })
                });
            } else {
                // Create new student
                await fetchWithAuth('/Students', {
                    method: 'POST',
                    body: JSON.stringify(studentData)
                });
            }
            
            // Refresh student list
            await loadStudentData();
            document.getElementById('student-form-modal').style.display = 'none';
        } catch (error) {
            console.error('Error saving student:', error);
            alert('Failed to save student data');
        }
    }

    async function loadCoursesSection(sectionElement) {
        sectionElement.innerHTML = `
            <h2>Course Management</h2>
            <div class="action-bar">
                <button id="add-course-btn" class="btn-primary">Add New Course</button>
                <div class="search-box">
                    <input type="text" id="course-search" placeholder="Search courses...">
                    <button id="search-course-btn" class="btn">Search</button>
                </div>
            </div>
            <div class="table-container">
                <table id="courses-table">
                    <thead>
                        <tr>
                            <th>Course Code</th>
                            <th>Course Name</th>
                            <th>Credit Hours</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody id="courses-table-body">
                        <!-- Courses will be loaded here -->
                    </tbody>
                </table>
            </div>
            <div id="course-form-modal" class="modal">
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3 id="course-form-title">Add New Course</h3>
                    <form id="course-form">
                        <input type="hidden" id="course-id">
                        <div class="form-group">
                            <label for="course-code">Course Code</label>
                            <input type="text" id="course-code" required>
                        </div>
                        <div class="form-group">
                            <label for="course-name">Course Name</label>
                            <input type="text" id="course-name" required>
                        </div>
                        <div class="form-group">
                            <label for="credit-hours">Credit Hours</label>
                            <input type="number" id="credit-hours" required min="1">
                        </div>
                        <div class="form-group">
                            <label for="course-description">Description</label>
                            <textarea id="course-description" rows="3"></textarea>
                        </div>
                        <button type="submit" class="btn-primary">Save</button>
                    </form>
                </div>
            </div>
        `;
        
        // Load course data
        await loadCourseData();
        
        // Add event listeners
        document.getElementById('add-course-btn').addEventListener('click', () => {
            openCourseForm();
        });
        
        document.getElementById('course-form').addEventListener('submit', handleCourseFormSubmit);
        
        document.querySelector('.close-modal').addEventListener('click', () => {
            document.getElementById('course-form-modal').style.display = 'none';
        });
        
        document.getElementById('search-course-btn').addEventListener('click', async (e) => {
            e.preventDefault();
            const searchTerm = document.getElementById('course-search').value.toLowerCase();
            await loadCourseData(searchTerm);
        });
    }

    async function loadCourseData(searchTerm = '') {
        try {
            let url = '/Courses';
            if (searchTerm) {
                url += `?search=${encodeURIComponent(searchTerm)}`;
            }
            
            state.courses = await fetchWithAuth(url);
            renderCourseTable();
        } catch (error) {
            console.error('Error loading courses:', error);
            alert('Failed to load course data');
        }
    }

    function renderCourseTable() {
        const tbody = document.getElementById('courses-table-body');
        tbody.innerHTML = '';
        
        state.courses.forEach(course => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${course.code}</td>
                <td>${course.name}</td>
                <td>${course.creditHours}</td>
                <td>${course.isActive ? 'Active' : 'Inactive'}</td>
                <td>
                    <button class="btn edit-course" data-id="${course.id}">Edit</button>
                    <button class="btn view-students" data-id="${course.id}">View Students</button>
                </td>
            `;
            
            tbody.appendChild(tr);
        });
        
        // Add event listeners to buttons
        document.querySelectorAll('.edit-course').forEach(btn => {
            btn.addEventListener('click', () => {
                const courseId = parseInt(btn.dataset.id);
                editCourse(courseId);
            });
        });
        
        document.querySelectorAll('.view-students').forEach(btn => {
            btn.addEventListener('click', async () => {
                const courseId = parseInt(btn.dataset.id);
                await viewCourseStudents(courseId);
            });
        });
    }

    function openCourseForm(course = null) {
        const modal = document.getElementById('course-form-modal');
        const form = document.getElementById('course-form');
        const title = document.getElementById('course-form-title');
        
        if (course) {
            title.textContent = 'Edit Course';
            document.getElementById('course-id').value = course.id;
            document.getElementById('course-code').value = course.code;
            document.getElementById('course-name').value = course.name;
            document.getElementById('credit-hours').value = course.creditHours;
            document.getElementById('course-description').value = course.description || '';
        } else {
            title.textContent = 'Add New Course';
            form.reset();
        }
        
        modal.style.display = 'block';
    }

    async function editCourse(courseId) {
        try {
            const course = await fetchWithAuth(`/Courses/${courseId}`);
            if (course) {
                openCourseForm(course);
            }
        } catch (error) {
            console.error('Error editing course:', error);
            alert('Failed to load course data');
        }
    }

    async function viewCourseStudents(courseId) {
        try {
            // In a full implementation, we would fetch students enrolled in this course
            const students = await fetchWithAuth('/Students');
            
            // For demo purposes, we'll just show a list of students
            const studentList = students.map(s => `${s.firstName} ${s.lastName}`).join(', ');
            alert(`Students enrolled in this course: ${studentList || 'None'}`);
        } catch (error) {
            console.error('Error viewing course students:', error);
            alert('Failed to load student data');
        }
    }

    async function handleCourseFormSubmit(e) {
        e.preventDefault();
        
        const courseId = document.getElementById('course-id').value;
        const courseData = {
            code: document.getElementById('course-code').value,
            name: document.getElementById('course-name').value,
            creditHours: parseInt(document.getElementById('credit-hours').value),
            description: document.getElementById('course-description').value
        };
        
        try {
            if (courseId) {
                // Update existing course
                await fetchWithAuth(`/Courses/${courseId}`, {
                    method: 'PUT',
                    body: JSON.stringify(courseData)
                });
            } else {
                // Create new course
                await fetchWithAuth('/Courses', {
                    method: 'POST',
                    body: JSON.stringify(courseData)
                });
            }
            
            // Refresh course list
            await loadCourseData();
            document.getElementById('course-form-modal').style.display = 'none';
        } catch (error) {
            console.error('Error saving course:', error);
            alert('Failed to save course data');
        }
    }

    async function loadTimetableSection(sectionElement) {
        sectionElement.innerHTML = `
            <h2>Timetable Management</h2>
            <div class="action-bar">
                <button id="add-timetable-btn" class="btn-primary">Add Timetable Slot</button>
                <div class="filter-options">
                    <select id="timetable-filter">
                        <option value="all">All Timetable</option>
                        <option value="current">Current Semester</option>
                        <option value="by-course">By Course</option>
                    </select>
                </div>
            </div>
            <div class="table-container">
                <table id="timetable-table">
                    <thead>
                        <tr>
                            <th>Course</th>
                            <th>Day</th>
                            <th>Time</th>
                            <th>Room</th>
                            <th>Academic Year</th>
                            <th>Semester</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody id="timetable-table-body">
                        <!-- Timetable will be loaded here -->
                    </tbody>
                </table>
            </div>
            <div class="no-print">
                <button id="print-timetable-btn" class="btn-primary">Print Timetable</button>
            </div>
            <div id="timetable-form-modal" class="modal">
                <div class="modal-content">
                    <span class="close-modal">&times;</span>
                    <h3 id="timetable-form-title">Add Timetable Slot</h3>
                    <form id="timetable-form">
                        <input type="hidden" id="timetable-id">
                        <div class="form-group">
                            <label for="timetable-course">Course</label>
                            <select id="timetable-course" required>
                                <option value="">Select Course</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label for="timetable-day">Day</label>
                            <select id="timetable-day" required>
                                <option value="">Select Day</option>
                                <option value="1">Monday</option>
                                <option value="2">Tuesday</option>
                                <option value="3">Wednesday</option>
                                <option value="4">Thursday</option>
                                <option value="5">Friday</option>
                                <option value="6">Saturday</option>
                                <option value="7">Sunday</option>
                            </select>
                        </div>
                        <div class="form-group">
                            <label for="timetable-start">Start Time</label>
                            <input type="time" id="timetable-start" required>
                        </div>
                        <div class="form-group">
                            <label for="timetable-end">End Time</label>
                            <input type="time" id="timetable-end" required>
                        </div>
                        <div class="form-group">
                            <label for="timetable-room">Room Number</label>
                            <input type="text" id="timetable-room">
                        </div>
                        <div class="form-group">
                            <label for="timetable-year">Academic Year</label>
                            <input type="text" id="timetable-year" required>
                        </div>
                        <div class="form-group">
                            <label for="timetable-semester">Semester</label>
                            <select id="timetable-semester" required>
                                <option value="">Select Semester</option>
                                <option value="First">First</option>
                                <option value="Second">Second</option>
                                <option value="Third">Third</option>
                            </select>
                        </div>
                        <button type="submit" class="btn-primary">Save</button>
                    </form>
                </div>
            </div>
        `;
        
        // Load timetable data
        await loadTimetableData();
        
        // Add event listeners
        document.getElementById('add-timetable-btn').addEventListener('click', async () => {
            await openTimetableForm();
        });
        
        document.getElementById('timetable-form').addEventListener('submit', handleTimetableFormSubmit);
        
        document.querySelector('.close-modal').addEventListener('click', () => {
            document.getElementById('timetable-form-modal').style.display = 'none';
        });
        
        document.getElementById('print-timetable-btn').addEventListener('click', () => {
            window.print();
        });
        
        document.getElementById('timetable-filter').addEventListener('change', async (e) => {
            await loadTimetableData(e.target.value);
        });
    }

    async function loadTimetableData(filter = 'all') {
        try {
            let url = '/Timetable';
            if (filter === 'current') {
                url += '?current=true';
            }
            
            state.timetable = await fetchWithAuth(url);
            renderTimetableTable();
        } catch (error) {
            console.error('Error loading timetable:', error);
            alert('Failed to load timetable data');
        }
    }

    function renderTimetableTable() {
        const tbody = document.getElementById('timetable-table-body');
        tbody.innerHTML = '';
        
        state.timetable.forEach(slot => {
            const course = state.courses.find(c => c.id === slot.courseId);
            const dayName = DAY_NAMES[slot.dayOfWeek];
            
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${course ? course.name : 'Unknown Course'}</td>
                <td>${dayName}</td>
                <td>${slot.startTime} - ${slot.endTime}</td>
                <td>${slot.roomNumber || ''}</td>
                <td>${slot.academicYear}</td>
                <td>${slot.semester}</td>
                <td>
                    <button class="btn edit-slot" data-id="${slot.id}">Edit</button>
                    <button class="btn delete-slot" data-id="${slot.id}">Delete</button>
                </td>
            `;
            
            tbody.appendChild(tr);
        });
        
        // Add event listeners to buttons
        document.querySelectorAll('.edit-slot').forEach(btn => {
            btn.addEventListener('click', async () => {
                const slotId = parseInt(btn.dataset.id);
                await editTimetableSlot(slotId);
            });
        });
        
        document.querySelectorAll('.delete-slot').forEach(btn => {
            btn.addEventListener('click', async () => {
                const slotId = parseInt(btn.dataset.id);
                if (confirm('Are you sure you want to delete this timetable slot?')) {
                    await deleteTimetableSlot(slotId);
                }
            });
        });
    }

    async function openTimetableForm(slot = null) {
        const modal = document.getElementById('timetable-form-modal');
        const form = document.getElementById('timetable-form');
        const title = document.getElementById('timetable-form-title');
        const courseSelect = document.getElementById('timetable-course');
        
        // Load courses into dropdown
        try {
            const courses = await fetchWithAuth('/Courses');
            courseSelect.innerHTML = '<option value="">Select Course</option>';
            
            courses.forEach(course => {
                const option = document.createElement('option');
                option.value = course.id;
                option.textContent = `${course.code} - ${course.name}`;
                courseSelect.appendChild(option);
            });
            
            if (slot) {
                title.textContent = 'Edit Timetable Slot';
                document.getElementById('timetable-id').value = slot.id;
                document.getElementById('timetable-course').value = slot.courseId;
                document.getElementById('timetable-day').value = slot.dayOfWeek;
                document.getElementById('timetable-start').value = slot.startTime;
                document.getElementById('timetable-end').value = slot.endTime;
                document.getElementById('timetable-room').value = slot.roomNumber || '';
                document.getElementById('timetable-year').value = slot.academicYear;
                document.getElementById('timetable-semester').value = slot.semester;
            } else {
                title.textContent = 'Add Timetable Slot';
                form.reset();
                // Set current academic year as default
                const currentYear = new Date().getFullYear();
                document.getElementById('timetable-year').value = `${currentYear}/${currentYear + 1}`;
            }
            
            modal.style.display = 'block';
        } catch (error) {
            console.error('Error loading courses:', error);
            alert('Failed to load course data');
        }
    }

    async function editTimetableSlot(slotId) {
        try {
            const slot = await fetchWithAuth(`/Timetable/${slotId}`);
            if (slot) {
                await openTimetableForm(slot);
            }
        } catch (error) {
            console.error('Error editing timetable slot:', error);
            alert('Failed to load timetable data');
        }
    }

    async function deleteTimetableSlot(slotId) {
        try {
            await fetchWithAuth(`/Timetable/${slotId}`, {
                method: 'DELETE'
            });
            
            // Refresh timetable
            await loadTimetableData();
        } catch (error) {
            console.error('Error deleting timetable slot:', error);
            alert('Failed to delete timetable slot');
        }
    }

    async function handleTimetableFormSubmit(e) {
        e.preventDefault();
        
        const slotId = document.getElementById('timetable-id').value;
        const slotData = {
            courseId: parseInt(document.getElementById('timetable-course').value),
            dayOfWeek: parseInt(document.getElementById('timetable-day').value),
            startTime: document.getElementById('timetable-start').value,
            endTime: document.getElementById('timetable-end').value,
            roomNumber: document.getElementById('timetable-room').value || null,
            academicYear: document.getElementById('timetable-year').value,
            semester: document.getElementById('timetable-semester').value
        };
        
        try {
            if (slotId) {
                // Update existing slot
                await fetchWithAuth(`/Timetable/${slotId}`, {
                    method: 'PUT',
                    body: JSON.stringify(slotData)
                });
            } else {
                // Create new slot
                await fetchWithAuth('/Timetable', {
                    method: 'POST',
                    body: JSON.stringify(slotData)
                });
            }
            
            // Refresh timetable
            await loadTimetableData();
            document.getElementById('timetable-form-modal').style.display = 'none';
        } catch (error) {
            console.error('Error saving timetable slot:', error);
            alert('Failed to save timetable slot');
        }
    }

    async function handleLogin(e) {
        e.preventDefault();
        
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        
        try {
            const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });
            
            if (!response.ok) {
                throw new Error('Login failed');
            }
            
            const data = await response.json();
            
           
            sessionStorage.setItem('authToken', data.token);
            sessionStorage.setItem('currentUser', JSON.stringify({
                username: data.username,
                role: data.role
            }));
            
            state.currentUser = {
                username: data.username,
                role: data.role
            };
            state.authToken = data.token;
            
            renderAuthenticatedUI();
        } catch (error) {
            alert('Login failed. Please check your credentials.');
            console.error('Login error:', error);
        }
    }

    function handleLogout() {
        state.currentUser = null;
        state.authToken = null;
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('authToken');
        renderLoginUI();
    }

    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    }

    // Modal functionality
    window.addEventListener('click', (e) => {
        const modals = [
            document.getElementById('student-form-modal'),
            document.getElementById('course-assignment-modal'),
            document.getElementById('course-form-modal'),
            document.getElementById('timetable-form-modal')
        ];
        
        modals.forEach(modal => {
            if (e.target === modal) {
                modal.style.display = 'none';
            }
        });
    });
});
           