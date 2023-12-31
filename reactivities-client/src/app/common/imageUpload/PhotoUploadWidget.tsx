import { Button, Grid, Header } from 'semantic-ui-react';
import PhotoWidgetDropzone from './PhotoWidgetDropzone';
import { useEffect, useState } from 'react';
import PhotoWidgetCropper from './PhotoWidgetCropper';

interface Props {
  loading: boolean;
  uploadPhoto: (file: Blob) => void;
}

const PhotoUploadWidget = ({ loading, uploadPhoto }: Props) => {
  // We're adding the preview property in the useCallback in PhotoWidgetDropzone
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const [files, setFiles] = useState<any>([]);

  const [cropper, setCropper] = useState<Cropper>();

  const onCrop = () => {
    if (cropper) {
      cropper.getCroppedCanvas().toBlob(blob => uploadPhoto(blob!));
    }
  };

  useEffect(() => {
    return () => {
      files.forEach((file: object & { preview?: string }) =>
        URL.revokeObjectURL(file.preview!)
      );
    };
  }, [files]);

  return (
    <Grid>
      <Grid.Column width={4}>
        <Header sub color='teal' content='step 1 - Add photo' />
        <PhotoWidgetDropzone setFiles={setFiles} />
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='step 2 - Resize & Crop' />
        {files && files.length > 0 && (
          <PhotoWidgetCropper
            setCropper={setCropper}
            imagePreview={files[0].preview}
          />
        )}
      </Grid.Column>
      <Grid.Column width={1} />
      <Grid.Column width={4}>
        <Header sub color='teal' content='step 3 - Preview & Finish' />
        {files && files.length > 0 && (
          <>
            <div
              className='img-preview'
              style={{ minHeight: 200, overflow: 'hidden' }}
            />
            <Button.Group>
              <Button
                loading={loading}
                onClick={onCrop}
                positive
                icon='check'
              />
              <Button
                disabled={loading}
                onClick={() => setFiles([])}
                icon='close'
              />
            </Button.Group>
          </>
        )}
      </Grid.Column>
    </Grid>
  );
};
export default PhotoUploadWidget;
