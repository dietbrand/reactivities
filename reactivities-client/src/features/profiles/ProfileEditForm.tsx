import { Formik, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { useStore } from '../../app/stores/store';
import MyTextInput from '../../app/common/form/MyTextInput';
import MyTextArea from '../../app/common/form/MyTextArea';
import ValidationError from '../errors/ValidationError';
import { Button } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';

interface Props {
  setEditMode: (editMode: boolean) => void;
}

const ProfileEditForm = ({ setEditMode }: Props) => {
  const {
    profileStore: { profile, editProfile },
  } = useStore();
  return (
    <Formik
      initialValues={{
        displayName: profile?.displayName,
        bio: profile?.bio,
        error: null,
      }}
      onSubmit={(values, { setErrors }) => {
        editProfile(values)
          .then(() => setEditMode(false))
          .catch(error => setErrors({ error }));
      }}
      validationSchema={Yup.object({
        displayName: Yup.string().required(),
      })}
    >
      {({ isSubmitting, errors, isValid, dirty }) => (
        <Form className='ui form error'>
          <MyTextInput placeholder='Display name' name='displayName' />
          <MyTextArea placeholder='Your bio goes here' name='bio' rows={3} />
          <ErrorMessage
            name='error'
            render={() => (
              <ValidationError errors={errors.error as unknown as string[]} />
            )}
          />
          <Button
            disabled={!isValid || !dirty || isSubmitting}
            loading={isSubmitting}
            positive
            content='Save'
            type='submit'
            floates='right'
          />
        </Form>
      )}
    </Formik>
  );
};
export default observer(ProfileEditForm);
