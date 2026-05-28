import { useState } from 'react';
import axiosClient from '../api/axiosClient';

export default function GestionAgents() {
  const [agentData, setAgentData] = useState({ nom: '', cin: '' });
  const [qrCode, setQrCode] = useState(null);
  const [loading, setLoading] = useState(false);

  const createAgent = async () => {
    if (!agentData.nom || !agentData.cin) {
      alert("Fenoy ny anarana sy ny CIN!");
      return;
    }

    setLoading(true);
    try {
      const { data } = await axiosClient.post('/Users/create-agent', agentData);
      setQrCode(data.qrCodeUrl); // URL avy amin'ny backend
      alert("Voaforona ny kaonty Agent!");
    } catch (err) {
      console.error(err);
      alert("Nisy olana: " + (err.response?.data || "Tsy nahomby ny famoronana Agent"));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '20px', maxWidth: '400px' }}>
      <h2>Recrutement Agent</h2>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
        <input 
          placeholder="Anarana" 
          value={agentData.nom}
          onChange={e => setAgentData({...agentData, nom: e.target.value})} 
        />
        <input 
          placeholder="CIN" 
          value={agentData.cin}
          onChange={e => setAgentData({...agentData, cin: e.target.value})} 
        />
        <button onClick={createAgent} disabled={loading}>
          {loading ? "Mamorona..." : "Mamorona Compte"}
        </button>
      </div>
      
      {qrCode && (
        <div style={{ marginTop: '20px', border: '1px solid #ddd', padding: '10px', textAlign: 'center' }}>
          <h3>QR Code ho an'ny Agent:</h3>
          <img src={qrCode} alt="Agent QR" style={{ width: '200px', height: '200px' }} />
          <p style={{ fontSize: '0.8em', color: '#666' }}>
            *Alefaso any amin'ny Agent ity QR code ity mba hahafahany miditra (Log-in).
          </p>
        </div>
      )}
    </div>
  );
}