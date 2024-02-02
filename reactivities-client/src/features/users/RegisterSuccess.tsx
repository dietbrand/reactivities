import { toast } from 'react-toastify';
import agent from '../../app/api/agent';
import useQuery from '../../app/util/hooks';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';

const RegisterSuccess = () => {
  const email = useQuery().get('email') as string;

  const handleConfirmEmailResend = () => {
    agent.Account.resendEmailConfirmation(email)
      .then(() => {
        toast.success('Verification email resent. Please check your inbox.');
      })
      .catch(error => console.log(error));
  };

  return (
    <Segment placeholder textAlign='center'>
      <Header icon color='green'>
        <Icon name='check' /> Registered successfully.
      </Header>
      <p>Please check your inbox for the verification mail.</p>
      {email && (
        <>
          <p>Didn't receive the mail? Click on the button to send it again.</p>
          <Button
            primary
            onClick={handleConfirmEmailResend}
            content='Resend'
            size='huge'
          />
        </>
      )}
    </Segment>
  );
};
export default RegisterSuccess;
