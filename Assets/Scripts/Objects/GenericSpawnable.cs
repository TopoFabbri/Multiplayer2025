namespace Objects
{
    public class GenericSpawnable : SpawnableObject
    {
        private void Update()
        {
            objectManager.UpdatePosition(ID, transform.position);
        }
    }
}