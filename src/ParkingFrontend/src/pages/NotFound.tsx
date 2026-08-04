import { Link } from "react-router-dom";

export const NotFound = () => {
    return (
        <div style={{ textAlign: 'center', marginTop: '50px' }}>
          <h2>404 - Page Not Found</h2>
          <p>The page you are looking for does not exist.</p>
          <Link to="/" style={{ color: 'blue', textDecoration: 'underline' }}>
            Return to Dashboard
          </Link>
        </div>
  );
}