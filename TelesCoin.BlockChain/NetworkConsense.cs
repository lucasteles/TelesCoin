namespace TelesCoin
{
    public class NetworkConsense
    {
        public int Difficulty { get; }
        public int BlockSize { get; }
        public double MiningReward { get; }

        public NetworkConsense(int difficulty, int blockSize, int miningReward)
        {
            Difficulty = difficulty;
            BlockSize = blockSize;
            MiningReward = miningReward;
        }

    }
}
