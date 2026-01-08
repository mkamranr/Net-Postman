// NetPostman - Main Application JavaScript

// Global state
let state = {
    activeEnvironment: null,
    environments: [],
    currentRequest: {
        method: 'GET',
        url: '',
        headers: [],
        params: [],
        bodyType: 'none',
        body: '',
        preRequestScript: '',
        testScript: '',
        collectionId: null,
        requestId: null
    }
};

// Initialize CodeMirror editors
let bodyEditor, preRequestEditor, testsEditor;

document.addEventListener('DOMContentLoaded', function() {
    initializeEditors();
    loadCollections();
    loadEnvironments();
    loadHistory();
    
    // Add default header
    addKeyValueRow('headers-list', 'Content-Type', 'application/json');
});

function initializeEditors() {
    // Body editor
    const bodyEditorElement = document.getElementById('body-editor');
    if (bodyEditorElement) {
        bodyEditor = CodeMirror.fromTextArea(bodyEditorElement, {
            mode: 'application/json',
            theme: 'dracula',
            lineNumbers: true,
            lineWrapping: true,
            indentUnit: 2,
            tabSize: 2
        });
    }
    
    // Pre-request script editor
    const preRequestElement = document.getElementById('pre-request-editor');
    if (preRequestElement) {
        preRequestEditor = CodeMirror.fromTextArea(preRequestElement, {
            mode: 'javascript',
            theme: 'dracula',
            lineNumbers: true,
            lineWrapping: true,
            indentUnit: 2,
            tabSize: 2
        });
    }
    
    // Tests editor
    const testsElement = document.getElementById('tests-editor');
    if (testsElement) {
        testsEditor = CodeMirror.fromTextArea(testsElement, {
            mode: 'javascript',
            theme: 'dracula',
            lineNumbers: true,
            lineWrapping: true,
            indentUnit: 2,
            tabSize: 2
        });
    }
}

// Tab switching
function switchTab(tabName) {
    document.querySelectorAll('.tab-btn').forEach(btn => {
        btn.classList.remove('active');
        if (btn.dataset.tab === tabName) {
            btn.classList.add('active');
        }
    });
    
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.remove('active');
    });
    document.getElementById('tab-' + tabName).classList.add('active');
}

// Request tab switching
function switchRequestTab(tabName) {
    document.querySelectorAll('.request-tab').forEach(tab => {
        tab.classList.remove('active');
        if (tab.dataset.tab === tabName) {
            tab.classList.add('active');
        }
    });
    
    document.querySelectorAll('.request-tab-panel').forEach(panel => {
        panel.classList.remove('active');
    });
    document.getElementById('tab-' + tabName).classList.add('active');
}

// Response tab switching
function switchResponseTab(tabName) {
    document.querySelectorAll('.response-tab').forEach(tab => {
        tab.classList.remove('active');
        if (tab.dataset.tab === tabName) {
            tab.classList.add('active');
        }
    });
    
    document.querySelectorAll('.response-tab-panel').forEach(panel => {
        panel.classList.remove('active');
    });
    document.getElementById('tab-' + tabName).classList.add('active');
}

// Update method color
function updateMethodColor() {
    const method = document.getElementById('request-method').value;
    const select = document.querySelector('.method-select');
    select.style.color = getMethodColor(method);
}

function getMethodColor(method) {
    const colors = {
        'GET': '#61affe',
        'POST': '#49cc90',
        'PUT': '#fca130',
        'PATCH': '#50e3c2',
        'DELETE': '#f93e3e',
        'HEAD': '#9012fe',
        'OPTIONS': '#0d5aa7'
    };
    return colors[method] || '#cccccc';
}

// Add key-value row
function addKeyValueRow(containerId, key = '', value = '') {
    const container = document.getElementById(containerId);
    if (!container) return;
    
    const row = document.createElement('div');
    row.className = 'kv-row';
    row.innerHTML = `
        <input type="text" placeholder="Key" value="${escapeHtml(key)}" onchange="updateKVRow(this)">
        <input type="text" placeholder="Value" value="${escapeHtml(value)}" onchange="updateKVRow(this)">
        <input type="checkbox" checked title="Enabled">
        <button class="btn-icon" onclick="removeKVRow(this)" title="Remove">
            <svg width="14" height="14" viewBox="0 0 14 14" fill="currentColor">
                <path d="M12 3.5L10.5 2 6 6.5 1.5 2 0 3.5 4.5 8 0 12.5 1.5 14 6 9.5 10.5 14 12 12.5 7.5 8 12 3.5z"/>
            </svg>
        </button>
    `;
    container.appendChild(row);
}

function addFormDataRow(key = '', value = '', type = 'text') {
    const container = document.getElementById('form-data-list');
    if (!container) return;
    
    const row = document.createElement('div');
    row.className = 'kv-row';
    row.innerHTML = `
        <input type="text" placeholder="Key" value="${escapeHtml(key)}">
        <input type="text" placeholder="Value" value="${escapeHtml(value)}">
        <select class="kv-type-select">
            <option value="text" ${type === 'text' ? 'selected' : ''}>Text</option>
            <option value="file" ${type === 'file' ? 'selected' : ''}>File</option>
        </select>
        <button class="btn-icon" onclick="removeKVRow(this)">
            <svg width="14" height="14" viewBox="0 0 14 14" fill="currentColor">
                <path d="M12 3.5L10.5 2 6 6.5 1.5 2 0 3.5 4.5 8 0 12.5 1.5 14 6 9.5 10.5 14 12 12.5 7.5 8 12 3.5z"/>
            </svg>
        </button>
    `;
    container.appendChild(row);
}

function removeKVRow(button) {
    button.closest('.kv-row').remove();
}

function updateKVRow(input) {
    // Placeholder for any row update logic
}

// Update body type
function updateBodyType(type) {
    const bodyEditorContainer = document.getElementById('body-editor-container');
    const formDataContainer = document.getElementById('body-form-data-container');
    
    state.currentRequest.bodyType = type;
    
    if (type === 'raw' || type === 'urlencoded') {
        bodyEditorContainer.style.display = 'block';
        formDataContainer.style.display = 'none';
        updateBodyContentType();
    } else if (type === 'form-data') {
        bodyEditorContainer.style.display = 'none';
        formDataContainer.style.display = 'block';
    } else {
        bodyEditorContainer.style.display = 'none';
        formDataContainer.style.display = 'none';
    }
}

function updateBodyContentType() {
    const contentType = document.getElementById('body-content-type').value;
    const mode = contentType === 'json' ? 'application/json' :
                 contentType === 'xml' ? 'xml' :
                 contentType === 'html' ? 'html' :
                 contentType === 'javascript' ? 'javascript' : 'text/plain';
    
    if (bodyEditor) {
        bodyEditor.setOption('mode', mode);
    }
}

// Update auth type
function updateAuthType(type) {
    document.getElementById('bearer-auth-container').style.display = type === 'bearer' ? 'block' : 'none';
    document.getElementById('basic-auth-container').style.display = type === 'basic' ? 'block' : 'none';
}

// Send request
async function sendRequest() {
    const method = document.getElementById('request-method').value;
    const url = document.getElementById('request-url').value.trim();
    
    if (!url) {
        showToast('Please enter a URL', 'error');
        return;
    }
    
    // Build headers
    const headers = [];
    document.querySelectorAll('#headers-list .kv-row').forEach(row => {
        const inputs = row.querySelectorAll('input[type="text"]');
        const checkbox = row.querySelector('input[type="checkbox"]');
        if (inputs[0].value && checkbox.checked) {
            headers.push({
                key: inputs[0].value,
                value: inputs[1].value,
                enabled: checkbox.checked
            });
        }
    });
    
    // Add auth header if configured
    const authType = document.querySelector('input[name="authType"]:checked').value;
    if (authType === 'bearer') {
        const token = document.getElementById('bearer-token').value;
        if (token) {
            headers.push({ key: 'Authorization', value: 'Bearer ' + token, enabled: true });
        }
    } else if (authType === 'basic') {
        const username = document.getElementById('basic-username').value;
        const password = document.getElementById('basic-password').value;
        if (username) {
            const credentials = btoa(username + ':' + password);
            headers.push({ key: 'Authorization', value: 'Basic ' + credentials, enabled: true });
        }
    }
    
    // Build body
    let body = null;
    const bodyType = state.currentRequest.bodyType;
    if (bodyType === 'raw') {
        body = bodyEditor ? bodyEditor.getValue() : document.getElementById('body-editor').value;
    } else if (bodyType === 'form-data') {
        const formData = {};
        document.querySelectorAll('#form-data-list .kv-row').forEach(row => {
            const inputs = row.querySelectorAll('input[type="text"]');
            const typeSelect = row.querySelector('select');
            if (inputs[0].value) {
                formData[inputs[0].value] = {
                    value: inputs[1].value,
                    type: typeSelect.value
                };
            }
        });
        body = JSON.stringify(formData);
    } else if (bodyType === 'urlencoded') {
        const urlencoded = {};
        document.querySelectorAll('#form-data-list .kv-row').forEach(row => {
            const inputs = row.querySelectorAll('input[type="text"]');
            if (inputs[0].value) {
                urlencoded[inputs[0].value] = inputs[1].value;
            }
        });
        body = new URLSearchParams(urlencoded).toString();
    }
    
    // Show loading state
    const sendBtn = document.getElementById('send-btn');
    const originalContent = sendBtn.innerHTML;
    sendBtn.innerHTML = '<span class="spinner"></span> Sending...';
    sendBtn.disabled = true;
    
    // Update response panel
    document.getElementById('response-status').innerHTML = '<span class="spinner"></span>';
    document.getElementById('response-status').className = 'status-badge pending';
    document.getElementById('response-body').textContent = '';
    document.getElementById('response-time').textContent = '';
    document.getElementById('response-size').textContent = '';
    
    try {
        const response = await fetch('/Home/ExecuteRequest', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                method,
                url,
                headers,
                body,
                bodyType,
                requestId: state.currentRequest.requestId,
                collectionId: state.currentRequest.collectionId
            })
        });
        
        const result = await response.json();
        
        // Update response display
        updateResponseDisplay(result);
        
        // Reload history
        loadHistory();
        
    } catch (error) {
        showToast('Error: ' + error.message, 'error');
        document.getElementById('response-status').textContent = 'Error';
        document.getElementById('response-status').className = 'status-badge error';
        document.getElementById('response-body').textContent = error.message;
    } finally {
        sendBtn.innerHTML = originalContent;
        sendBtn.disabled = false;
    }
}

function updateResponseDisplay(result) {
    const statusEl = document.getElementById('response-status');
    const timeEl = document.getElementById('response-time');
    const sizeEl = document.getElementById('response-size');
    const bodyEl = document.getElementById('response-body');
    
    if (result.success) {
        statusEl.textContent = `${result.statusCode} ${result.statusText || ''}`;
        statusEl.className = `status-badge ${result.isSuccess ? 'success' : 'error'}`;
        
        timeEl.textContent = `${result.responseTime}ms`;
        sizeEl.textContent = formatBytes(result.responseSize);
        
        bodyEl.textContent = result.body || '';
        
        // Format JSON response
        try {
            const parsed = JSON.parse(result.body);
            bodyEl.textContent = JSON.stringify(parsed, null, 2);
        } catch (e) {
            // Not JSON, keep as is
        }
        
        // Update cookies
        updateCookiesDisplay(result.cookies || []);
        
        // Update headers
        updateHeadersDisplay(result.headers || []);
        
        // Run tests
        runTests(result);
    } else {
        statusEl.textContent = 'Error';
        statusEl.className = 'status-badge error';
        bodyEl.textContent = result.errorMessage || 'Unknown error';
    }
}

function updateCookiesDisplay(cookies) {
    const container = document.getElementById('response-cookies');
    if (!cookies || cookies.length === 0) {
        container.innerHTML = '<div class="empty-state">No cookies in response</div>';
        return;
    }
    
    container.innerHTML = cookies.map(cookie => `
        <div class="cookie-item">
            <span class="cookie-name">${escapeHtml(cookie.name)}</span>
            <span class="cookie-value">${escapeHtml(cookie.value)}</span>
        </div>
    `).join('');
}

function updateHeadersDisplay(headers) {
    const container = document.getElementById('response-headers');
    if (!headers || headers.length === 0) {
        container.innerHTML = '<div class="empty-state">No headers</div>';
        return;
    }
    
    container.innerHTML = headers.map(header => `
        <div class="header-item">
            <span class="header-name">${escapeHtml(header.key)}</span>
            <span class="header-value">${escapeHtml(header.value)}</span>
        </div>
    `).join('');
}

function runTests(result) {
    const container = document.getElementById('test-results');
    
    // Basic response validation
    const tests = [];
    
    // Status code test
    tests.push({
        name: 'Status code is 200',
        pass: result.statusCode === 200
    });
    
    // Response time test
    tests.push({
        name: 'Response time < 1000ms',
        pass: result.responseTime < 1000
    });
    
    // Custom tests from editor
    const testScript = state.currentRequest.testScript;
    if (testScript) {
        try {
            // Create test environment
            const testEnv = {
                response: {
                    status: result.statusCode,
                    body: result.body,
                    headers: result.headers,
                    time: result.responseTime,
                    size: result.responseSize,
                    to: {
                        have: {
                            status: (code) => ({ pass: result.statusCode === code })
                        }
                    }
                },
                test: (name, fn) => {
                    try {
                        fn();
                        tests.push({ name, pass: true });
                    } catch (e) {
                        tests.push({ name, pass: false, error: e.message });
                    }
                }
            };
            
            // Execute script (simplified)
            const testFn = new Function('pm', testScript);
            testFn(testEnv);
        } catch (e) {
            tests.push({ name: 'Custom tests', pass: false, error: e.message });
        }
    }
    
    container.innerHTML = tests.map(test => `
        <div class="test-item ${test.pass ? 'pass' : 'fail'}">
            <svg class="test-icon ${test.pass ? 'pass' : 'fail'}" width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                ${test.pass 
                    ? '<path d="M13.78 4.22a.75.75 0 0 1 0 1.06l-7.25 7.25a.75.75 0 0 1-1.06 0L2.22 9.28a.75.75 0 0 1 1.06-1.06L6 10.94l6.72-6.72a.75.75 0 0 1 1.06 0z"/>'
                    : '<path d="M3.72 3.72a.75.75 0 0 1 1.06 0L8 6.94l3.22-3.22a.75.75 0 0 1 1.06 1.06L9.06 8l3.22 3.22a.75.75 0 0 1-1.06 1.06L8 9.06l-3.22 3.22a.75.75 0 0 1-1.06-1.06L6.94 8 3.72 4.78a.75.75 0 0 1 0-1.06z"/>'}
            </svg>
            <span class="test-name">${escapeHtml(test.name)}</span>
            ${test.error ? `<span class="test-error">${escapeHtml(test.error)}</span>` : ''}
        </div>
    `).join('');
}

function copyResponse() {
    const body = document.getElementById('response-body').textContent;
    navigator.clipboard.writeText(body).then(() => {
        showToast('Response copied to clipboard', 'success');
    });
}

function clearResponse() {
    document.getElementById('response-body').textContent = '';
    document.getElementById('response-status').textContent = '';
    document.getElementById('response-status').className = 'status-badge';
    document.getElementById('response-time').textContent = '';
    document.getElementById('response-size').textContent = '';
    document.getElementById('response-cookies').innerHTML = '';
    document.getElementById('response-headers').innerHTML = '';
    document.getElementById('test-results').innerHTML = '';
}

// Save request
async function saveRequest() {
    if (!state.currentRequest.collectionId) {
        showToast('Please select or create a collection first', 'warning');
        return;
    }
    
    const requestName = prompt('Enter request name:', state.currentRequest.requestName || 'New Request');
    if (!requestName) return;
    
    const headers = [];
    document.querySelectorAll('#headers-list .kv-row').forEach(row => {
        const inputs = row.querySelectorAll('input[type="text"]');
        if (inputs[0].value) {
            headers.push({
                key: inputs[0].value,
                value: inputs[1].value,
                enabled: row.querySelector('input[type="checkbox"]').checked
            });
        }
    });
    
    const body = bodyEditor ? bodyEditor.getValue() : document.getElementById('body-editor').value;
    
    try {
        const response = await fetch('/Collection/SaveRequest', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ...state.currentRequest,
                requestName,
                headers,
                body: state.currentRequest.bodyType === 'raw' ? body : undefined,
                preRequestScript: preRequestEditor ? preRequestEditor.getValue() : document.getElementById('pre-request-editor').value,
                testScript: testsEditor ? testsEditor.getValue() : document.getElementById('tests-editor').value
            })
        });
        
        const result = await response.json();
        if (result.success) {
            state.currentRequest.requestId = result.id;
            showToast('Request saved successfully', 'success');
            loadCollections();
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error saving request: ' + error.message, 'error');
    }
}

// Load collections
async function loadCollections() {
    try {
        const response = await fetch('/Collection/GetCollections');
        const result = await response.json();
        
        if (result.success) {
            renderCollections(result.collections);
        }
    } catch (error) {
        console.error('Error loading collections:', error);
    }
}

function renderCollections(collections) {
    const container = document.getElementById('collections-list');
    
    if (!collections || collections.length === 0) {
        container.innerHTML = '<div class="empty-state">No collections yet</div>';
        return;
    }
    
    container.innerHTML = collections.map(collection => `
        <div class="collection-item">
            <div class="collection-header" onclick="toggleCollection('${collection.id}')">
                <svg width="14" height="14" viewBox="0 0 14 14" fill="currentColor" class="collection-chevron">
                    <path d="M4.33 2.5L9.33 7 4.33 11.5 3.33 10.5 7.33 7 3.33 3.5z"/>
                </svg>
                <svg width="14" height="14" viewBox="0 0 14 14" fill="currentColor">
                    <path d="M12 2H2a2 2 0 0 0-2 2v8a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V4a2 2 0 0 0-2-2zm0 10H2V4h10v8z"/>
                </svg>
                <span class="collection-name">${escapeHtml(collection.name)}</span>
                <div class="collection-actions">
                    <button class="btn-icon" onclick="event.stopPropagation(); addRequestToCollection('${collection.id}')" title="Add request">
                        <svg width="12" height="12" viewBox="0 0 12 12" fill="currentColor">
                            <path d="M6 1v10M1 6h10"/>
                        </svg>
                    </button>
                    <button class="btn-icon" onclick="event.stopPropagation(); deleteCollection('${collection.id}')" title="Delete">
                        <svg width="12" height="12" viewBox="0 0 12 12" fill="currentColor">
                            <path d="M3 2a1 1 0 0 0-1 1v1H1a1 1 0 0 0 0 2h1v7a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V6h1a1 1 0 0 0 0-2h-1V3a1 1 0 0 0-1-1H3zm2 1h6v1H5V3z"/>
                        </svg>
                    </button>
                </div>
            </div>
            <div class="collection-requests">
                ${collection.requests && collection.requests.length > 0 ? collection.requests.map(request => `
                    <div class="request-item ${state.currentRequest.requestId === request.requestId ? 'active' : ''}" 
                         onclick="loadRequest('${request.requestId}')">
                        <span class="method-badge ${request.method}">${request.method}</span>
                        <span class="request-url">${escapeHtml(request.name)}</span>
                    </div>
                `).join('') : ''}
            </div>
        </div>
    `).join('');
}

function toggleCollection(id) {
    // Toggle collection expansion
}

async function addRequestToCollection(collectionId) {
    state.currentRequest.collectionId = collectionId;
    showToast('Request will be saved to collection', 'success');
}

async function deleteCollection(id) {
    if (!confirm('Are you sure you want to delete this collection?')) return;
    
    try {
        const response = await fetch('/Collection/Delete?id=' + id, { method: 'POST' });
        const result = await response.json();
        if (result.success) {
            loadCollections();
            showToast('Collection deleted', 'success');
        } else {
            showToast(result.message, 'error');
        }
    } catch (error) {
        showToast('Error deleting collection', 'error');
    }
}

async function loadRequest(requestId) {
    try {
        const response = await fetch('/Collection/GetRequest?id=' + requestId);
        const result = await response.json();
        
        if (result.success) {
            const request = result.request;
            
            state.currentRequest.requestId = request.requestId;
            state.currentRequest.requestName = request.name;
            state.currentRequest.collectionId = request.collectionId;
            state.currentRequest.method = request.method;
            state.currentRequest.url = request.url;
            state.currentRequest.bodyType = request.bodyType || 'none';
            state.currentRequest.body = request.body || '';
            state.currentRequest.preRequestScript = request.preRequestScript || '';
            state.currentRequest.testScript = request.testScript || '';
            
            // Update UI
            document.getElementById('request-method').value = request.method;
            document.getElementById('request-url').value = request.url;
            updateMethodColor();
            
            // Update body
            updateBodyType(request.bodyType || 'none');
            if (bodyEditor && request.body) {
                bodyEditor.setValue(request.body);
            }
            
            // Update pre-request and test scripts
            if (preRequestEditor && request.preRequestScript) {
                preRequestEditor.setValue(request.preRequestScript);
            }
            if (testsEditor && request.testScript) {
                testsEditor.setValue(request.testScript);
            }
            
            // Update headers
            document.getElementById('headers-list').innerHTML = '';
            if (request.headers && request.headers.length > 0) {
                request.headers.forEach(h => addKeyValueRow('headers-list', h.key, h.value));
            }
            
            // Reload collections to update active state
            loadCollections();
        }
    } catch (error) {
        showToast('Error loading request', 'error');
    }
}

// Load history
async function loadHistory() {
    try {
        const response = await fetch('/History/GetHistory?limit=30');
        const result = await response.json();
        
        if (result.success) {
            renderHistory(result.history);
        }
    } catch (error) {
        console.error('Error loading history:', error);
    }
}

function renderHistory(history) {
    const container = document.getElementById('history-list');
    
    if (!history || history.length === 0) {
        container.innerHTML = '<div class="empty-state">No history yet</div>';
        return;
    }
    
    container.innerHTML = history.map(entry => `
        <div class="history-item" onclick="loadHistoryEntry('${entry.id}')">
            <span class="method-badge ${entry.method}">${entry.method}</span>
            <div style="flex: 1; min-width: 0;">
                <div class="history-url">${escapeHtml(entry.url)}</div>
                <div class="history-time">${formatTime(entry.executedAt)}</div>
            </div>
            ${entry.statusCode ? `<span class="status-badge ${entry.statusCode >= 200 && entry.statusCode < 300 ? 'success' : 'error'}">${entry.statusCode}</span>` : ''}
        </div>
    `).join('');
}

async function loadHistoryEntry(id) {
    try {
        const response = await fetch('/History/GetHistoryEntry?id=' + id);
        const result = await response.json();
        
        if (result.success && result.entry) {
            const entry = result.entry;
            
            document.getElementById('request-method').value = entry.method;
            document.getElementById('request-url').value = entry.url;
            updateMethodColor();
            
            // Clear previous response
            clearResponse();
            
            // Show response
            document.getElementById('response-status').textContent = `${entry.statusCode} ${entry.statusText || ''}`;
            document.getElementById('response-status').className = `status-badge ${entry.statusCode >= 200 && entry.statusCode < 300 ? 'success' : 'error'}`;
            document.getElementById('response-time').textContent = `${entry.responseTime}ms`;
            document.getElementById('response-size').textContent = formatBytes(entry.responseSize || 0);
            document.getElementById('response-body').textContent = entry.responseBody || '';
        }
    } catch (error) {
        showToast('Error loading history entry', 'error');
    }
}

async function clearHistory() {
    if (!confirm('Are you sure you want to clear all history?')) return;
    
    try {
        const response = await fetch('/History/ClearHistory', { method: 'POST' });
        const result = await response.json();
        if (result.success) {
            loadHistory();
            showToast('History cleared', 'success');
        }
    } catch (error) {
        showToast('Error clearing history', 'error');
    }
}

// Load environments
async function loadEnvironments() {
    try {
        const response = await fetch('/Environment/GetEnvironments');
        const result = await response.json();
        
        if (result.success) {
            state.environments = result.environments || [];
            renderEnvironments();
        }
    } catch (error) {
        console.error('Error loading environments:', error);
    }
}

function renderEnvironments() {
    const select = document.getElementById('active-environment');
    const container = document.getElementById('environments-list');
    
    const options = '<option value="">No Environment</option>' + 
        state.environments.map(e => `<option value="${e.id}" ${state.activeEnvironment === e.id ? 'selected' : ''}>${escapeHtml(e.name)}</option>`).join('');
    
    select.innerHTML = options;
    
    if (container) {
        container.innerHTML = state.environments.map(e => `
            <div class="environment-item" onclick="changeEnvironment('${e.id}')">
                <svg width="14" height="14" viewBox="0 0 14 14" fill="currentColor">
                    <path d="M7 1a6 6 0 0 0-6 6v1H1a1 1 0 0 0-1 1v6a1 1 0 0 0 1 1h1v1a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1v-1h1a1 1 0 0 0 1-1v-6a1 1 0 0 0-1-1H8V7a6 6 0 0 0-6-6zm0 2a4 4 0 0 1 4 4v3H3V7a4 4 0 0 1 4-4z"/>
                </svg>
                <span>${escapeHtml(e.name)}</span>
            </div>
        `).join('') || '<div class="empty-state">No environments</div>';
    }
}

function changeEnvironment(id) {
    state.activeEnvironment = id;
    const select = document.getElementById('active-environment');
    select.value = id;
    
    if (id) {
        const env = state.environments.find(e => e.id === id);
        if (env) {
            showToast(`Switched to ${env.name} environment`, 'success');
        }
    }
}

// Modals
function showCreateCollectionModal() {
    showModal('Create Collection', `
        <div class="form-group">
            <label>Name</label>
            <input type="text" id="collection-name" placeholder="My Collection">
        </div>
        <div class="form-group">
            <label>Description (optional)</label>
            <textarea id="collection-description" placeholder="Collection description"></textarea>
        </div>
    `, async () => {
        const name = document.getElementById('collection-name').value.trim();
        if (!name) {
            showToast('Please enter a collection name', 'error');
            return;
        }
        
        try {
            const response = await fetch('/Collection/Create', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    name,
                    description: document.getElementById('collection-description').value
                })
            });
            
            const result = await response.json();
            if (result.success) {
                closeModal();
                loadCollections();
                showToast('Collection created', 'success');
            } else {
                showToast(result.message, 'error');
            }
        } catch (error) {
            showToast('Error creating collection', 'error');
        }
    });
}

function showCreateEnvironmentModal() {
    showModal('Create Environment', `
        <div class="form-group">
            <label>Name</label>
            <input type="text" id="env-name" placeholder="My Environment">
        </div>
        <div class="form-group">
            <label>Variables (JSON)</label>
            <textarea id="env-variables" style="min-height: 150px; font-family: monospace;">{
    "baseUrl": "https://api.example.com",
    "apiKey": "your-api-key"
}</textarea>
        </div>
    `, async () => {
        const name = document.getElementById('env-name').value.trim();
        if (!name) {
            showToast('Please enter an environment name', 'error');
            return;
        }
        
        try {
            const variablesText = document.getElementById('env-variables').value;
            let variables;
            try {
                variables = JSON.parse(variablesText);
            } catch (e) {
                showToast('Invalid JSON in variables', 'error');
                return;
            }
            
            const response = await fetch('/Environment/Create', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    name,
                    variables
                })
            });
            
            const result = await response.json();
            if (result.success) {
                closeModal();
                loadEnvironments();
                showToast('Environment created', 'success');
            } else {
                showToast(result.message, 'error');
            }
        } catch (error) {
            showToast('Error creating environment', 'error');
        }
    });
}

function showModal(title, content, onConfirm) {
    document.getElementById('modal-title').textContent = title;
    document.getElementById('modal-body').innerHTML = content;
    document.getElementById('modal-overlay').style.display = 'flex';
    
    // Store confirm handler
    window.modalConfirmHandler = onConfirm;
}

function closeModal(event) {
    if (event && event.target !== event.currentTarget) return;
    
    document.getElementById('modal-overlay').style.display = 'none';
    window.modalConfirmHandler = null;
}

function confirmModal() {
    if (window.modalConfirmHandler) {
        window.modalConfirmHandler();
    }
}

// Toast notifications
function showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `
        <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
            ${type === 'success' 
                ? '<path d="M13.78 4.22a.75.75 0 0 1 0 1.06l-7.25 7.25a.75.75 0 0 1-1.06 0L2.22 9.28a.75.75 0 0 1 1.06-1.06L6 10.94l6.72-6.72a.75.75 0 0 1 1.06 0z"/>'
                : type === 'error'
                ? '<path d="M3.72 3.72a.75.75 0 0 1 1.06 0L8 6.94l3.22-3.22a.75.75 0 0 1 1.06 1.06L9.06 8l3.22 3.22a.75.75 0 0 1-1.06 1.06L8 9.06l-3.22 3.22a.75.75 0 0 1-1.06-1.06L6.94 8 3.72 4.78a.75.75 0 0 1 0-1.06z"/>'
                : '<path d="M8 1a7 7 0 1 0 0 14A7 7 0 0 0 8 1zm0 12a5 5 0 1 1 0-10 5 5 0 0 1 0 10zm.5-8.5v3h-3v-3H8l3 3 3-3h-2.5z"/>'}
        </svg>
        <span>${escapeHtml(message)}</span>
    `;
    
    container.appendChild(toast);
    
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Utility functions
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatBytes(bytes) {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

function formatTime(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now - date;
    
    if (diff < 60000) return 'Just now';
    if (diff < 3600000) return Math.floor(diff / 60000) + 'm ago';
    if (diff < 86400000) return Math.floor(diff / 3600000) + 'h ago';
    return date.toLocaleDateString();
}

function toggleSidebar() {
    document.getElementById('sidebar').classList.toggle('collapsed');
}

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + Enter to send request
    if ((e.ctrlKey || e.metaKey) && e.key === 'Enter') {
        e.preventDefault();
        sendRequest();
    }
});
