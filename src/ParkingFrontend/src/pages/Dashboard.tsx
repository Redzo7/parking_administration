import { useState } from 'react';
import { useParkingSlots } from '../query/useParkingSlots';
import { useUsers } from '../query/useUsers';
import { Modal } from '../components/Modal';
import { ReservationForm } from '../components/ReservationForm';
import { useUser } from '../context/UserContext';
import { useCancelReservation } from '../query/useCancelReservation';
import { SlotType } from '../models/types';
import { getSlotEmoji } from '../utils/formatters';

export const Dashboard = () => {
  const { data: slots, isLoading: isLoadingSlots, isError, error } = useParkingSlots();
  const { data: users } = useUsers(); // Fetch users to check active user's permissions
  const { selectedUserId } = useUser();
  const cancelReservation = useCancelReservation();
  
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedSlot, setSelectedSlot] = useState<{ id: string; designation: string } | null>(null);

  // Find the currently active user to verify their authorizations
  const activeUser = users?.find(u => u.id === selectedUserId);

  if (isLoadingSlots) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#4b5563' }}><h2>Loading parking slots...</h2></div>;
  if (isError) return <div style={{ textAlign: 'center', marginTop: '3rem', color: '#ef4444' }}><h2>Failed to load data</h2><p>{error?.message}</p></div>;

  const handleOpenModal = (id: string, designation: string) => {
    setSelectedSlot({ id, designation });
    setIsModalOpen(true);
  };

  const handleCloseModal = () => {
    setIsModalOpen(false);
    setSelectedSlot(null);
  };

  const handleCancel = (reservationId: string) => {
    if (window.confirm('Are you sure you want to cancel this reservation?')) {
      cancelReservation.mutate(reservationId, {
        onError: (err) => {
          alert(err.response?.data?.message || 'Failed to cancel reservation');
        }
      });
    }
  };

  return (
    <div>
      <h2 style={{ borderBottom: '2px solid #e5e7eb', paddingBottom: '0.5rem' }}>
        Parking Slots Availability
      </h2>
      
      <div style={{ 
        display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', 
        gap: '1.5rem', marginTop: '1.5rem'
      }}>
        {slots?.map((slot) => {
          // UI Logic: Check if the user is authorized to book this slot type
          const isAuthorized = slot.type === SlotType.Regular || (activeUser?.authorizedSlotTypes.includes(slot.type) ?? false);
          const slotEmoji = getSlotEmoji(slot.type);

          return (
            <div key={slot.id} style={{
              border: '1px solid #d1d5db', borderRadius: '8px', padding: '1.5rem',
              backgroundColor: '#f9fafb', boxShadow: '0 2px 4px rgba(0,0,0,0.05)'
            }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h3 style={{ margin: 0, color: '#1f2937' }}>
                  Slot: {slot.designation} <span title="Slot Type">{slotEmoji}</span>
                </h3>
                
                <button 
                  onClick={() => handleOpenModal(slot.id, slot.designation)}
                  disabled={!isAuthorized}
                  title={!isAuthorized ? 'You do not have the required authorization to book this slot.' : 'Book this slot'}
                  style={{
                    padding: '0.5rem 1rem', 
                    backgroundColor: isAuthorized ? '#10b981' : '#d1d5db', 
                    color: isAuthorized ? 'white' : '#6b7280', 
                    border: 'none', borderRadius: '4px', 
                    cursor: isAuthorized ? 'pointer' : 'not-allowed', 
                    fontWeight: 600, fontSize: '0.875rem'
                  }}
                >
                  Book Slot
                </button>
              </div>
              
              {slot.reservations.length === 0 ? (
                <p style={{ color: '#6b7280', fontStyle: 'italic', margin: 0 }}>Available / No reservations.</p>
              ) : (
                <ul style={{ listStyleType: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
                  {slot.reservations.map((res) => {
                    const startLocal = new Date(res.startTime).toLocaleString();
                    const endLocal = new Date(res.endTime).toLocaleString();
                    
                    const isFuture = new Date(res.startTime) > new Date();
                    const isOwner = res.userId === selectedUserId;
                    const canCancel = isFuture && isOwner;
                    const isCancelingThis = cancelReservation.isPending && cancelReservation.variables === res.id;

                    return (
                      <li key={res.id} style={{
                        backgroundColor: '#ffffff', border: '1px solid #e5e7eb', padding: '0.75rem',
                        borderRadius: '6px', fontSize: '0.875rem', color: '#374151',
                        display: 'flex', justifyContent: 'space-between', alignItems: 'center'
                      }}>
                        <div>
                          <div style={{ marginBottom: '4px' }}><span style={{ fontWeight: 600, color: '#10b981' }}>Start:</span> {startLocal}</div>
                          <div><span style={{ fontWeight: 600, color: '#ef4444' }}>End:</span> {endLocal}</div>
                        </div>
                        
                        {canCancel && (
                          <button
                            onClick={() => handleCancel(res.id)}
                            disabled={isCancelingThis}
                            style={{
                              padding: '0.4rem 0.75rem',
                              backgroundColor: isCancelingThis ? '#f3f4f6' : '#fee2e2',
                              color: isCancelingThis ? '#9ca3af' : '#b91c1c',
                              border: `1px solid ${isCancelingThis ? '#d1d5db' : '#fca5a5'}`,
                              borderRadius: '4px',
                              cursor: isCancelingThis ? 'not-allowed' : 'pointer',
                              fontWeight: 600,
                              fontSize: '0.75rem',
                              transition: 'background-color 0.2s'
                            }}
                          >
                            {isCancelingThis ? 'Canceling...' : 'Cancel'}
                          </button>
                        )}
                      </li>
                    );
                  })}
                </ul>
              )}
            </div>
          );
        })}
      </div>

      <Modal isOpen={isModalOpen} onClose={handleCloseModal}>
        {selectedSlot && (
          <ReservationForm 
            parkingSlotId={selectedSlot.id} 
            designation={selectedSlot.designation} 
            onClose={handleCloseModal} 
          />
        )}
      </Modal>
    </div>
  );
};