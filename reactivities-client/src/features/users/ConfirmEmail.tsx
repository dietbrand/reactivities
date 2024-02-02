import { useEffect, useState } from 'react';
import { useStore } from '../../app/stores/store';
import useQuery from '../../app/util/hooks';
import agent from '../../app/api/agent';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';
import LoginForm from './LoginForm';
import { toast } from 'react-toastify';

const ConfirmEmail = () => {
  const { modalStore } = useStore();
  const email = useQuery().get('email') as string;
  const token = useQuery().get('token') as string;

  const Status = {
    Verifying: 'Verifying',
    Failed: 'Failed',
    Success: 'Success',
  };

  const [status, setStatus] = useState(Status.Verifying);

  const handleConfirmEmailResend = () => {
    agent.Account.resendEmailConfirmation(email)
      .then(() => {
        toast.success('Verification email resent. Please check your inbox.');
      })
      .catch(error => console.log(error));
  };

  useEffect(() => {
    agent.Account.verifyEmail(token, email)
      .then(() => {
        setStatus(Status.Success);
      })
      .catch(() => {
        setStatus(Status.Failed);
      });
  }, [Status.Failed, Status.Success, email, token]);

  const getBody = () => {
    switch (status) {
      case Status.Verifying:
        return <p>Verifying...</p>;
      case Status.Failed:
        return (
          <div>
            <p>Verification failed. Try resending the verify link.</p>
            <Button
              primary
              onClick={handleConfirmEmailResend}
              size='huge'
              content='Resend'
            />
          </div>
        );
      case Status.Success:
        return (
          <div>
            <p>Email verified. Click the button to login</p>
            <Button
              primary
              onClick={() => modalStore.openModal(<LoginForm />)}
              size='huge'
              content='Login'
            />
          </div>
        );
    }
  };

  return (
    <Segment placeholder textAlign='center'>
      <Header icon>
        <Icon name='envelope' />
        Email verification
      </Header>
      <Segment.Inline>{getBody()}</Segment.Inline>
    </Segment>
  );
};
export default ConfirmEmail;
