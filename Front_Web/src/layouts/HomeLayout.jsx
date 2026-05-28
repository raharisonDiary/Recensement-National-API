import { Outlet, Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useEffect, useState } from 'react';

export default function HomeLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [pendingCount, setPendingCount] = useState(0);

  // Fanamarihana: .toLowerCase() mba tsy ho diso na Admin na admin
  const role = user?.role?.toLowerCase();

  useEffect(() => {
    const checkPending = () => {
      const m = JSON.parse(localStorage.getItem('pending_menages') || '[]');
      const c = JSON.parse(localStorage.getItem('pending_citoyens') || '[]');
      setPendingCount(m.length + c.length);
    };
    checkPending();
    const interval = setInterval(checkPending, 5000);
    return () => clearInterval(interval);
  }, []);

  const handleLogout = () => { logout(); navigate('/login'); };

  return (
    <div className="layout" style={{ display: 'flex', minHeight: '100vh' }}>
      <nav style={{ width: '250px', background: '#f4f4f4', padding: '20px', borderRight: '1px solid #ddd' }}>
        <h2>Recensement</h2>
        <ul style={{ listStyle: 'none', padding: 0, display: 'flex', flexDirection: 'column', gap: '10px' }}>
          <li><Link to="/dashboard">Dashboard</Link></li>
          
          {/* Menu Agent */}
          {role === 'agent' && (
            <>
              <li><Link to="/menages">Fampidirana Ménage</Link></li>
              <li><Link to="/citoyens">Fampidirana Citoyen</Link></li>
            </>
          )}

          {/* Sync & Statistique (Ho an'ny rehetra) */}
          <li><Link to="/sync">Sync Data ({pendingCount})</Link></li>
          <li><Link to="/statistiques">Statistique</Link></li>

          {/* Menu Regional & Admin */}
          {(role === 'regional' || role === 'admin') && (
            <>
              <li><Link to="/rapports">Fandefasana Rapport</Link></li>
              <li><Link to="/gestion-agents">Gestion Agents</Link></li>
            </>
          )}

          {/* Menu Admin ihany */}
          {role === 'admin' && (
            <li><Link to="/gestion-regionaux">Gestion Régionaux</Link></li>
          )}
        </ul>
        
        <div style={{ marginTop: 'auto', paddingTop: '20px' }}>
          <p>Profil: <strong>{user?.nom}</strong> ({role})</p>
          <button onClick={handleLogout} style={{ color: 'red' }}>Déconnexion</button>
        </div>
      </nav>

      <main style={{ flex: 1, padding: '20px' }}><Outlet /></main>
    </div>
  );
}