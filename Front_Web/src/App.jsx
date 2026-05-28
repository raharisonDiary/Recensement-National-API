import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import ProtectedRoute from './components/ProtectedRoute';
import HomeLayout from './layouts/HomeLayout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import GestionMenage from './pages/GestionMenage';
import GestionCitoyen from './pages/GestionCitoyen';
import Statistique from './pages/Statistique';
import GestionAgents from './pages/GestionAgents';
import GestionRegionaux from './pages/GestionRegionaux';
import RapportPage from './pages/RapportPage';
import Unauthorized from './pages/Unauthorized';
import SyncPage from './pages/SyncPage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/unauthorized" element={<Unauthorized />} />

        {/* Ny ProtectedRoute lehibe no miaro ny zava-drehetra */}
        <Route path="/" element={<ProtectedRoute><HomeLayout /></ProtectedRoute>}>
          <Route index element={<Dashboard />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="menages" element={<GestionMenage />} />
          <Route path="citoyens" element={<GestionCitoyen />} />
          <Route path="sync" element={<SyncPage />} />
          <Route path="statistiques" element={<Statistique />} />
          <Route path="gestion-agents" element={<GestionAgents />} />
          <Route path="rapports" element={<RapportPage />} />
          <Route path="gestion-regionaux" element={<GestionRegionaux />} />
        </Route>

        <Route path="*" element={<Navigate to="/dashboard" />} />
      </Routes>
    </Router>
  );
}
export default App;