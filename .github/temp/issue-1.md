Create a React component for the retirement calculator input form with all MVP fields and client-side validation.

## User Story
As a user, I want to enter my retirement information through an intuitive form so that I can calculate my retirement withdrawal strategy.

## Acceptance Criteria
- [ ] Create `InputForm.jsx` component in `frontend/src/components/`
- [ ] Implement all MVP input fields:
  - [ ] Current age (18-100)
  - [ ] Retirement age (>= current age)
  - [ ] Retirement account balance (401k/IRA)
  - [ ] Taxable account balance
  - [ ] Success rate threshold (dropdown: 90%, 95%, 98%)
- [ ] Add client-side validation:
  - [ ] Age must be 18-100
  - [ ] Retirement age must be >= current age
  - [ ] Balance fields must be positive numbers
  - [ ] All fields are required
- [ ] Display validation errors in real-time
- [ ] Add tooltips/help text for technical terms
- [ ] Create form submission handler
- [ ] Add loading state during API call
- [ ] Style the form with consistent UI design

## Technical Details
- Use React hooks (useState, useEffect)
- Use controlled components for all inputs
- Integrate with `apiClient.js` for form submission
- Handle errors gracefully with error messages
- Format currency inputs with proper separators

## Files to Create/Modify
- `frontend/src/components/InputForm.jsx` (new)
- `frontend/src/components/InputForm.css` (new, optional)
- `frontend/src/App.jsx` (modify to include InputForm)

## Dependencies
None (can be developed independently)

## Testing
- Test all validation rules
- Test form submission with valid data
- Test error handling with invalid data
- Test loading states
