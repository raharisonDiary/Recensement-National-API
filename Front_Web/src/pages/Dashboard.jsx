import { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import axiosClient from '../api/axiosClient';

export default function Dashboard() {
  const { user } = useAuth();
  const [stats, setStats] = useState(null);

  useEffect(() => {
    // Maka statistika raha tsy Agent
    if (user?.role !== 'Agent') {
      axiosClient.get('/Statistiques/global')
        .then(res => setStats(res.data))
        .catch(err => console.error("Tsy nahazo stats:", err));
    }
  }, [user]);

  return (
    <div className="dashboard" style={{ padding: '20px' }}>
      <h1>Tongasoa eto, {user?.nom}</h1>
      <p>Role: <strong>{user?.role}</strong></p>

      {/* Statistika ho an'ny Admin sy Regional */}
      {stats && (
        <div style={{ display: 'flex', gap: '20px', marginTop: '20px' }}>
          <div style={cardStyle}><h3>Total Citoyens</h3><p>{stats.totalPopulation}</p></div>
          <div style={cardStyle}><h3>Lahy</h3><p>{stats.lahy}</p></div>
          <div style={cardStyle}><h3>Vavy</h3><p>{stats.vavy}</p></div>
        </div>
      )}

      {/* Menu Actions araka ny Role */}
      <div className="actions" style={{ marginTop: '30px' }}>
        {user?.role === 'Agent' && (
          <div style={actionBox}>
            <h3>Asa miandry:</h3>
            <button onClick={() => window.location.href='/menages'}>+ Inscription Menage</button>
          </div>
        )}
        
        {user?.role === 'Regional' && (
          <div style={actionBox}>
            <h3>Gestion:</h3>
            <button onClick={() => window.location.href='/gestion-agents'}>Jereo ny Agents</button>
            <button onClick={() => window.location.href='/rapports'}>Alefaso Rapport</button>
          </div>
        )}

        {user?.role === 'Admin' && (
          <div style={actionBox}>
            <h3>Supervision:</h3>
            <button onClick={() => window.location.href='/gestion-regionaux'}>Gestion Régionaux</button>
          </div>
        )}
      </div>
    </div>
  );
}

// Styles tsotra ho an'ny dashboard
const cardStyle = { padding: '15px', border: '1px solid #ccc', borderRadius: '8px', minWidth: '150px' };
const actionBox = { padding: '20px', backgroundColor: '#f4f4f4', borderRadius: '8px' };