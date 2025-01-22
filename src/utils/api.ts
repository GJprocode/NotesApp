
// 'http://localhost:7065/api', Secure connection, correct certificate settings
// 'http://localhost:5000/api', Non Secure
// Import axios for making HTTP requests
import axios from 'axios';

// Create an Axios instance for API requests
const api = axios.create({
    baseURL: 'https://localhost:7065/api', // Backend's secure HTTPS URL
    headers: {
        'Content-Type': 'application/json', // Set JSON as the default content type
    },
    // Note: httpsAgent is unnecessary for frontend Axios requests
});

// Export the Axios instance for use in the application
export default api;

/**
 * Summary:
 * This file sets up a reusable Axios instance configured to communicate with the backend API.
 * - `baseURL`: Specifies the backend's API endpoint. It uses HTTPS for secure connections.
 * - `headers`: Sets `Content-Type` to `application/json` for JSON payloads by default.
 * This allows for consistent and streamlined API requests throughout the application.
 */



