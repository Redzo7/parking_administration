import { useState, useEffect } from 'react';

interface ReservationFormProps {
  parkingSlotId: string;
  designation: string;
  onClose: () => void;
}

export const ReservationForm = ({ parkingSlotId, designation, onClose }: ReservationFormProps) => {
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [validationError, setValidationError] = useState('');

  useEffect(() => {
    if (!startTime || !endTime) {
      setValidationError('');
      return;
    }

    const start = new Date(startTime);
    const end = new Date(endTime);
    const now = new Date();

    if (start < now) {
      setValidationError('Start time cannot be in the past.');
      return;
    }

    const diffInMilliseconds = end.getTime() - start.getTime();
    const diffInMinutes = diffInMilliseconds / (1000 * 60);

    if (diffInMinutes < 30) {
      setValidationError('End time must be at least 30 minutes after start time.');
      return;
    }

    setValidationError('');
  }, [startTime, endTime]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validationError || !startTime || !endTime) return;
    
    console.log('Validation passed! Ready to submit:', { parkingSlotId, startTime, endTime });
    alert('Form is valid! (API Mutation pending in the next task)');
  };

  const isFormValid = startTime && endTime && !validationError;

  return (
    <div>
      <h2 style={{ marginTop: 0, marginBottom: '0.5rem', color: '#111827' }}>Book Slot</h2>
      <p style={{ color: '#6b7280', marginBottom: '1.5rem' }}>
        Reserving slot: <strong>{designation}</strong>
      </p>

      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        
        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
          <label htmlFor="startTime" style={{ fontSize: '0.875rem', fontWeight: 600 }}>Start Time</label>
          <input
            type="datetime-local"
            id="startTime"
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
            style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #d1d5db' }}
            required
          />
        </div>

        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
          <label htmlFor="endTime" style={{ fontSize: '0.875rem', fontWeight: 600 }}>End Time</label>
          <input
            type="datetime-local"
            id="endTime"
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
            style={{ padding: '0.5rem', borderRadius: '4px', border: '1px solid #d1d5db' }}
            required
          />
        </div>

        {/* Validation Warning */}
        {validationError && (
          <div style={{ color: '#ef4444', fontSize: '0.875rem', fontWeight: 500 }}>
            {validationError}
          </div>
        )}

        <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '1rem', marginTop: '1rem' }}>
          <button
            type="button"
            onClick={onClose}
            style={{
              padding: '0.5rem 1rem', borderRadius: '4px', border: '1px solid #d1d5db',
              backgroundColor: '#ffffff', cursor: 'pointer'
            }}
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isFormValid}
            style={{
              padding: '0.5rem 1rem', borderRadius: '4px', border: 'none',
              backgroundColor: isFormValid ? '#3b82f6' : '#9ca3af',
              color: '#ffffff', cursor: isFormValid ? 'pointer' : 'not-allowed'
            }}
          >
            Submit
          </button>
        </div>
      </form>
    </div>
  );
};