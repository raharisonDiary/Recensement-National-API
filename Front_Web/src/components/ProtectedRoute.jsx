import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function ProtectedRoute({ children, allowedRoles }) {
  const { user } = useAuth();

  // 1. Raha tsy tafiditra (not logged in)
  if (!user) {
    return <Navigate to="/login" replace />;
  }

  // 2. Raha misy role voafetra (allowedRoles)
  // Eto isika manao .toLowerCase() mba tsy hisy olana amin'ny litera lehibe/kely
  if (allowedRoles) {
    const userRole = user.role ? user.role.toLowerCase() : '';
    const hasAccess = allowedRoles.some(role => role.toLowerCase() === userRole);

    if (!hasAccess) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  // 3. Raha afaka miditra
  return children;
}