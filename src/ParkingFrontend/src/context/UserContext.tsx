import { createContext, useState, type ReactNode, useContext, useEffect } from 'react';

interface UserContextType {
  selectedUserId: string;
  setSelectedUserId: (id: string) => void;
}

export const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: ReactNode }) => {
  // Initialize state from local storage if available
  const [selectedUserId, setSelectedUserId] = useState<string>(
    localStorage.getItem('selectedUserId') || ''
  );

  // Sync state to local storage whenever it changes
  useEffect(() => {
    if (selectedUserId) {
      localStorage.setItem('selectedUserId', selectedUserId);
    } else {
      localStorage.removeItem('selectedUserId');
    }
  }, [selectedUserId]);

  return (
    <UserContext.Provider value={{ selectedUserId, setSelectedUserId }}>
      {children}
    </UserContext.Provider>
  );
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUser must be used within a UserProvider');
  }
  return context;
};