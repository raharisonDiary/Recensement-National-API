import { createContext, useContext, useState, useCallback } from 'react';
import axiosClient from '../api/axiosClient';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    // Mampiasa sessionStorage fa tsy localStorage
    const savedUser = sessionStorage.getItem('user');
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const login = useCallback(async (credentials) => {
    const { data } = await axiosClient.post('/Users/login', credentials);
    const { token, ...userData } = data;
    
    // Mitahiry ao amin'ny sessionStorage
    sessionStorage.setItem('token', token);
    sessionStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
  }, []);

  const agentLogin = useCallback(async (qrCodeSecret) => {
    const { data } = await axiosClient.post('/Users/agent-login', JSON.stringify(qrCodeSecret), {
      headers: { 'Content-Type': 'application/json' }
    });
    const { token, ...userData } = data;
    
    sessionStorage.setItem('token', token);
    sessionStorage.setItem('user', JSON.stringify(userData));
    setUser(userData);
  }, []);

  const logout = useCallback(() => {
    // Fafao izay anao ihany fa aza atao .clear() mba tsy hamafa data hafa
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');
    setUser(null);
    window.location.href = '/login';
  }, []);

  return (
    <AuthContext.Provider value={{ user, login, agentLogin, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => useContext(AuthContext);