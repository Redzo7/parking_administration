import axios from 'axios';

export const axiosInstance = axios.create({
  baseURL: 'http://localhost:5000',
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config) => {
    // Read the current user ID from local storage
    const userId = localStorage.getItem('selectedUserId');

    // If we have an ID, inject the custom header
    if (userId) {
      config.headers['X-User-Id'] = userId;
    }

    return config;
  },
  (error) => {
    // Handle request errors here
    return Promise.reject(error);
  }
);