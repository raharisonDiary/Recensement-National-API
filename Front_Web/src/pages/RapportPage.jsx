import { useState } from 'react';
import axiosClient from '../api/axiosClient';

export default function RapportPage() {
  const [rapport, setRapport] = useState({ titre: '', contenu: '' });
  const [loading, setLoading] = useState(false);

  const sendRapport = async () => {
    if (!rapport.titre || !rapport.contenu) {
      alert("Fenoy ny lohateny sy ny votoatin'ny rapport!");
      return;
    }

    setLoading(true);
    try {
      await axiosClient.post('/Rapports', rapport);
      alert("Voafetra soa aman-tsara ny rapport!");
      setRapport({ titre: '', contenu: '' }); // Reset form
    } catch (err) {
      console.error(err);
      alert("Nisy olana nandritra ny fandefasana. Hamarino ny internet.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ padding: '20px', maxWidth: '500px' }}>
      <h2>Fandefasana Rapport (Regional)</h2>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
        <input 
          placeholder="Titry ny Rapport" 
          value={rapport.titre}
          onChange={e => setRapport({...rapport, titre: e.target.value})} 
          style={{ padding: '10px' }}
        />
        <textarea 
          placeholder="Votoatin'ny rapport..." 
          value={rapport.contenu}
          onChange={e => setRapport({...rapport, contenu: e.target.value})} 
          rows="5"
          style={{ padding: '10px', resize: 'vertical' }}
        />
        <button 
          onClick={sendRapport} 
          disabled={loading}
          style={{ padding: '10px', cursor: 'pointer' }}
        >
          {loading ? "Mandefa..." : "Alefa amin'ny Admin"}
        </button>
      </div>
    </div>
  );
}