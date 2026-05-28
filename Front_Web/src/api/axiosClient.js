import axios from 'axios';

const axiosClient = axios.create({
  baseURL: 'http://localhost:5255/api',
});

// Ity no manampy ny Token amin'ny Authorization Header
axiosClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
}, (error) => {
  return Promise.reject(error);
});

export default axiosClient;