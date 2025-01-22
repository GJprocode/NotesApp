
  // 'http://localhost:7065/api', Secure connection, correct certificate settings
  // 'http://localhost:5000/api', Non Secure

  import axios from 'axios';

  const api = axios.create({
      baseURL: 'https://localhost:7065/api', // Useing backend's HTTPS URL
      headers: {
          'Content-Type': 'application/json',
      },
      //   httpsAgent, No need for https.Agent in frontend
  });

  export default api;






