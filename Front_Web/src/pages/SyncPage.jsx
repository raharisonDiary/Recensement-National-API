import { useState } from 'react';
import axiosClient from '../api/axiosClient';

export default function SyncPage() {
  const [syncing, setSyncing] = useState(false);

  const handleSync = async () => {
    const pendingMenages = JSON.parse(localStorage.getItem('pending_menages') || '[]');
    const pendingCitoyens = JSON.parse(localStorage.getItem('pending_citoyens') || '[]');

    if (pendingMenages.length === 0 && pendingCitoyens.length === 0) {
      alert("Tsy misy data tokony halefa.");
      return;
    }

    setSyncing(true);
    try {
      await axiosClient.post('/Sync/upload-all', {
        menages: pendingMenages,
        citoyens: pendingCitoyens
      });

      localStorage.removeItem('pending_menages');
      localStorage.removeItem('pending_citoyens');
      alert("Vita soa aman-tsara ny synchronisation!");
    } catch (err) {
      // Eto isika mampiasa an'io err io mba tsy hitarain'ny ESLint intsony
      console.error("Sync error details:", err);
      alert("Nisy olana nandritra ny sync, miezaha indray.");
    } finally {
      setSyncing(false);
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h2>Synchronisation des données</h2>
      <p>Data miandry sync: 
        <strong> {JSON.parse(localStorage.getItem('pending_menages') || '[]').length} </strong> Ménages, 
        <strong> {JSON.parse(localStorage.getItem('pending_citoyens') || '[]').length} </strong> Citoyens
      </p>
      <button onClick={handleSync} disabled={syncing}>
        {syncing ? "Mampiditra data..." : "Alefa any amin'ny Server (Sync Now)"}
      </button>
    </div>
  );
}