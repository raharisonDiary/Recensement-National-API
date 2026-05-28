import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { Html5QrcodeScanner } from 'html5-qrcode';
import axiosClient from '../api/axiosClient';

export default function Login() {
  const { login } = useAuth();
  const [isQrMode, setIsQrMode] = useState(false);
  const [credentials, setCredentials] = useState({ cin: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  // 1. Logique Login Standard
  const handleStandardLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await login(credentials);
      // Mampiasa sessionStorage mba ho fafana ny session rehefa manidy browser
      window.location.href = '/dashboard';
    } catch {
      setError("Misy diso ny CIN na ny Password.");
    } finally {
      setLoading(false);
    }
  };

  // 2. Logique QR Scanner
  useEffect(() => {
    let scanner = null;
    if (isQrMode) {
      scanner = new Html5QrcodeScanner("reader", { 
        fps: 10, 
        qrbox: { width: 250, height: 250 },
        rememberLastUsedCamera: true
      });

      scanner.render(
        async (decodedText) => {
          await scanner.clear();
          setLoading(true);
          try {
            const { data } = await axiosClient.post('/Users/agent-login', JSON.stringify(decodedText), {
              headers: { 'Content-Type': 'application/json' }
            });
            
            // Fampiasana sessionStorage fa tsy localStorage
            sessionStorage.setItem('token', data.token);
            sessionStorage.setItem('user', JSON.stringify(data)); 
            
            window.location.href = '/dashboard';
          } catch {
            setError("QR Code tsy manan-kery.");
            setLoading(false);
          }
        },
        () => { /* Nesorina ny argument err tsy ampiasaina */ }
      );
    }
    
    return () => {
      if (scanner) {
        scanner.clear().catch(() => {});
      }
    };
  }, [isQrMode]);

  return (
    <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px', textAlign: 'center' }}>
      <h2>Recensement National</h2>
      <p>Fidirana amin'ny system</p>
      
      <button onClick={() => { setIsQrMode(!isQrMode); setError(''); }}>
        {isQrMode ? "Hiditra amin'ny CIN/MDP" : "Hiditra amin'ny QR Code (Agent)"}
      </button>

      {error && <p style={{ color: 'red', marginTop: '10px', fontWeight: 'bold' }}>{error}</p>}

      {!isQrMode ? (
        <form onSubmit={handleStandardLogin} style={{ display: 'flex', flexDirection: 'column', gap: '15px', marginTop: '20px' }}>
          <input 
            type="text" 
            placeholder="CIN" 
            onChange={(e) => setCredentials({...credentials, cin: e.target.value})} 
            required 
          />
          <input 
            type="password" 
            placeholder="Password" 
            onChange={(e) => setCredentials({...credentials, password: e.target.value})} 
            required 
          />
          <button type="submit" disabled={loading}>{loading ? "Miandry..." : "Miditra"}</button>
        </form>
      ) : (
        <div style={{ marginTop: '20px' }}>
          <p>Scan-o ny QR Code-nao:</p>
          <div id="reader" style={{ width: '100%' }}></div>
        </div>
      )}
    </div>
  );
}