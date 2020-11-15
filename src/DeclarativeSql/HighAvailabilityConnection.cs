using System;
using System.Data;



namespace DeclarativeSql
{
    /// <summary>
    /// Provides a database connection with high availability.
    /// </summary>
    public abstract class HighAvailabilityConnection : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets the connection string to the master DB.
        /// </summary>
        private string MasterConnectionString { get; }


        /// <summary>
        /// Gets the connection string to the slave DB.
        /// </summary>
        private string SlaveConnectionString { get; }


        /// <summary>
        /// Gets the connection to the master DB.
        /// </summary>
        public IDbConnection Master => this.GetConnection(ref this.master, this.MasterConnectionString, AvailabilityTarget.Master);
        private IDbConnection master = null;


        /// <summary>
        /// Gets the connection to the slave DB.
        /// </summary>
        public IDbConnection Slave
        {
            get
            {
                if (this.ForceMaster)
                    return this.Master;

                if (this.MasterConnectionString == this.SlaveConnectionString)
                    return this.Master;

                return this.GetConnection(ref this.slave, this.SlaveConnectionString, AvailabilityTarget.Slave);
            }
        }
        private IDbConnection slave = null;


        /// <summary>
        /// Gets whether to use the master DB forcibly.
        /// </summary>
        private bool ForceMaster { get; }


        /// <summary>
        /// Gets whether the resource has been released.
        /// </summary>
        private bool IsDisposed { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Creates instance.
        /// </summary>
        /// <param name="masterConnectionString">Connection string to the master DB.</param>
        /// <param name="slaveConnectionString">Connection string to the slave DB.</param>
        /// <param name="forceMaster">Use the master DB forcibly.</param>
        public HighAvailabilityConnection(string masterConnectionString, string slaveConnectionString, bool forceMaster = false)
        {
            this.MasterConnectionString = masterConnectionString ?? throw new ArgumentNullException(nameof(masterConnectionString));
            this.SlaveConnectionString = slaveConnectionString ?? throw new ArgumentNullException(nameof(slaveConnectionString));
            this.ForceMaster = forceMaster;
        }


        /// <summary>
        /// Destroy instance.
        /// </summary>
        ~HighAvailabilityConnection()
            => this.Dispose(false);
        #endregion


        #region IDisposable Support
        /// <summary>
        /// Release used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Release used resources.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                return;

            this.DisposeConnection(ref this.slave, AvailabilityTarget.Slave);
            this.DisposeConnection(ref this.master, AvailabilityTarget.Master);
            this.IsDisposed = true;
        }
        #endregion


        #region abstract / virtual
        /// <summary>
        /// Creates a connection from the specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection(string connectionString, AvailabilityTarget target);


        /// <summary>
        /// Called before the connection is opened.
        /// </summary>
        /// <param name="target"></param>
        protected virtual void OnOpen(AvailabilityTarget target){}


        /// <summary>
        /// Called when the connection is closed.
        /// </summary>
        /// <param name="target"></param>
        protected virtual void OnClose(AvailabilityTarget target){}
        #endregion


        #region Others
        /// <summary>
        /// Get a database connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="connectionString"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private ref IDbConnection GetConnection(ref IDbConnection connection, string connectionString, AvailabilityTarget target)
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException(target.ToString());

            if (connection is null)
            {
                this.OnOpen(AvailabilityTarget.Master);
                connection = this.CreateConnection(connectionString, target);
                connection.Open();
            }
            return ref connection;
        }


        /// <summary>
        /// Release the database connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="target"></param>
        private void DisposeConnection(ref IDbConnection connection, AvailabilityTarget target)
        {
            if (connection is null)
                return;

            connection.Dispose();
            connection = null;
            this.OnClose(target);
        }
        #endregion
    }
}
