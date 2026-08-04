import { Link, Outlet } from "react-router-dom";

export const Layout = () => {
return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <nav style={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center', 
        padding: '1rem 2rem', 
        backgroundColor: '#f3f4f6', 
        borderBottom: '1px solid #e5e7eb' 
      }}>
        <h2 style={{ margin: 0, color: '#111827' }}>Parking Admin</h2>
        <div style={{ display: 'flex', gap: '1rem' }}>
          <Link to="/" style={{ textDecoration: 'none', color: '#374151', fontWeight: 'bold' }}>
            Dashboard
          </Link>
        </div>
      </nav>

      <main style={{ padding: '2rem', flex: 1, backgroundColor: '#ffffff' }}>
        <Outlet />
      </main>
    </div>
  );
}