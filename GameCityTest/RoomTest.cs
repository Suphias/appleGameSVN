using System;
using Xunit;
using AntDesigner.NetCore.GameCity;
using Moq;

namespace GameCityTest
{

    public class RoomTest
    {
        IPlayerJoinRoom player1;
        IPlayerJoinRoom player2;
        Room _room;
        public RoomTest()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory1 = new Mock<IPlayerJoinRoom>();
            IPlayerFactory1.SetupAllProperties();
            player1 = IPlayerFactory1.Object;
            player1.Id = 1;
            Mock<IPlayerJoinRoom> IPlayerFactory2 = new Mock<IPlayerJoinRoom>();
            IPlayerFactory2.SetupAllProperties();
            player2 = IPlayerFactory2.Object;
            player2.Id = 2;
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            _room = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory1.Object);
        }
        [Theory(DisplayName = "��room�����n��IPlayer")]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(0)]
        public void AddPlayer_1(int n)
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            IPlayerFactory.Setup(p => p.DecutMoney(0,"")).Returns(true);
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            _room = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object);
            for (int i = 0; i < n; i++)
            {
                var player = IPlayerFactory.Object;
                _room.AddPlayer(player);
            }
            Assert.Equal(_room.Players.Count, 1);
        }
        [Fact(DisplayName = "roomɾ��Player1")]
        public void RemovePlayer_1()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            IPlayerFactory.Setup(pp => pp.DecutMoney(0,"")).Returns(true);
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object);
            var p1 = IPlayerFactory.Object;
            myRoom.AddPlayer(p1);
            myRoom.RemovePlayer(p1);
            Assert.Equal(0, myRoom.Players.Count);
        }
        [Theory(DisplayName = "roomɾ��Player3")]
        [InlineData(10, 5)]
        public void RemovePlayer_3(int n, int id_)
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            _room = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object);
            for (int i = 0; i < n; i++)
            {
                Mock<IPlayerJoinRoom> p = new Mock<IPlayerJoinRoom>();
                p.SetupAllProperties().SetupGet(a => a.Id).Returns(i);
                _room.AddPlayer(p.Object);
            }
            var index = 0;
            foreach (var item in _room.Players)
            {
                if (item.Id == id_)
                {
                    index = _room.Players.IndexOf(item);
                }
            }
            _room.RemovePlayer(_room.Players[index]);
            Assert.DoesNotContain(_room.Players, p => p.Id == id_);
        }
        [Theory(DisplayName = "roomɾ��Player")]
        [InlineData(10, 5)]
        public void RemovePlayer_4(int n, int id_)
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            // IPlayerFactory.Setup(pp => pp.DecutMoney(0)).Returns(true);
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();

            _room = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object);
            for (int i = 0; i < n; i++)
            {
                Mock<IPlayerJoinRoom> p = new Mock<IPlayerJoinRoom>();
                p.SetupAllProperties().SetupGet(a => a.Id).Returns(i);
                p.Setup(pp => pp.DecutMoney(0,"")).Returns(true);
                _room.AddPlayer(p.Object);
            }
            var index = 0;
            foreach (var item in _room.Players)
            {
                if (item.Id == id_)
                {
                    index = _room.Players.IndexOf(item);
                }
            }
            _room.RemovePlayer(_room.Players[index]);
            Assert.Contains(_room.Players, p => p.Id == id_ - 1);
        }
        [Theory]
        [InlineData("new")]
        [InlineData("")]
        public void ChanageManger_1(string newManager_)
        {
            Mock<IPlayerJoinRoom> newManager = new Mock<IPlayerJoinRoom>();
            newManager.SetupGet(p => p.WeixinName).Returns(newManager_);
            _room.ChanageManger(newManager.Object);
            Assert.Equal(newManager_, _room.RoomManager.WeixinName);
        }
        [Fact]
        public void Close_1()
        {
            _room.Close();
            Assert.False(_room.IsOpening);
        }
        [Fact]
        public void Open()
        {
            _room.Open();
            Assert.True(_room.IsOpening);
        }
        [Theory(DisplayName = "�������")]
        [InlineData("mm")]
        [InlineData("")]
        public void Encrypt_2(string encyptor_)
        {
            try
            {
                _room.Encrypt(encyptor_);
                Assert.NotEqual("", _room.SecretKey);
            }
            catch (Exception)
            {

                Assert.ThrowsAny<Exception>(() => _room.Encrypt(""));
            }
        }
        [Fact(DisplayName = "ȥ����������")]
        public void DeEncrypt()
        {
            _room.DeEncrypt();
            Assert.Empty(_room.SecretKey);
        }
        [Theory(DisplayName = "�ӳ����������")]
        [InlineData(30)]
        [InlineData(0)]
        [InlineData(-20)]
        public void ProLongTimeLimite_1(int minite_)
        {
            try
            {
                var pretime = _room.Timelimit;
                _room.ProlongTimelimit(minite_);
                Assert.True(_room.Timelimit > pretime);
            }
            catch (Exception)
            {
                Assert.Throws<Exception>(() => _room.ProlongTimelimit(minite_));
            }
        }
        [Theory(DisplayName = "���÷�����������")]
        [InlineData(10)]
        [InlineData(5)]
        [InlineData(1)]
        public void SetPlayCountTopLimit_1(int count_)
        {
            _room.SetPlayerCountTopLimit(count_);
            Assert.Equal(count_, _room.PlayerCountTopLimit);
        }
        [Theory(DisplayName = "�����������޲���С��1")]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(-2)]
        public void SetPlayCountTopLimit_2(int count_)
        {
            try
            {
                _room.SetPlayerCountTopLimit(count_);
                Assert.True(_room.PlayerCountTopLimit >= 1);
            }
            catch (Exception)
            {
                Assert.Throws<Exception>(() => _room.SetPlayerCountTopLimit(count_));
            }
        }
        [Fact(DisplayName = "���������������޲��ܼ���")]
        public void AddPlayer_overflow()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 1, IInningGameFactory.Object, IPlayerFactory.Object);
            Mock<IPlayerJoinRoom> IPlayerFactory2 = new Mock<IPlayerJoinRoom>();
            Assert.False(myRoom.AddPlayer(IPlayerFactory2.Object));
        }
        [Fact(DisplayName = "������Ҽ���ʧ���¼�")]
        public void AddPlayerFail()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 1, IInningGameFactory.Object, IPlayerFactory.Object);
            var raisEvent = false;

            myRoom.AddPlayerFailRoomFullEvent += delegate { raisEvent = true; };
            Mock<IPlayerJoinRoom> IPlayerFactory2 = new Mock<IPlayerJoinRoom>();
            myRoom.AddPlayer(IPlayerFactory2.Object);
            Assert.True(raisEvent);
        }
        [Fact(DisplayName = "��Ҽ���ɹ��¼�")]
        public void AddPlayerSuccess()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();

            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 2, IInningGameFactory.Object, IPlayerFactory.Object);
            var raisEvent = false;
            myRoom.AddPlayer_SuccessEvent += delegate { raisEvent = true; };
            Mock<IPlayerJoinRoom> IPlayerFactory2 = new Mock<IPlayerJoinRoom>();
            IPlayerFactory2.Setup(pp => pp.DecutMoney(0,"")).Returns(true);
            IPlayerFactory2.SetupGet<int>(pp => pp.Id).Returns(2);
            myRoom.AddPlayer(IPlayerFactory2.Object);
            Assert.True(raisEvent);
        }
        [Fact(DisplayName = "�۳������Ʊ_�Ƿ���")]
        public void AddPlayer_getTiket()
        {
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice,""));
            player.SetupGet<int>(p => p.Id).Returns(2);
            var p2 = player.Object;
            _room.AddPlayer(p2);
            player.Verify(p => p.DecutMoney(_room.TicketPrice,""));
        }
        [Fact(DisplayName = "�۳������Ʊʧ��")]
        public void AddPlayer_notGettiket()
        {
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice,"")).Returns(false);
            var p2 = player.Object;
            _room.AddPlayer(p2);
            Assert.DoesNotContain<IPlayerJoinRoom>(p2, _room.Players);
        }
        [Fact(DisplayName = "���㲻�ܽ��뷿��")]
        public void AddPlayer_account()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object, ticketPrice_: 50);
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice,"")).Returns(true);
            player.Setup<decimal>(p => p.Account).Returns(0);
            var p2 = player.Object;
            myRoom.AddPlayer(p2);
            Assert.DoesNotContain<IPlayerJoinRoom>(p2, myRoom.Players);
        }
        [Fact(DisplayName = "��ҽ������㴥���¼�_����")]
        public void AddPlayer_accountEvent()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object, ticketPrice_: 50);
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice,"")).Returns(true);
            player.Setup<decimal>(p => p.Account).Returns(0);
            var p2 = player.Object;
            bool accountNotEnough = false;
            myRoom.PlayCanNotPayTicketEvent += delegate { accountNotEnough = true; };
            myRoom.AddPlayer(p2);
            Assert.False(accountNotEnough);
        }
        [Fact(DisplayName = "��ҽ������㴥���¼�_�Ƿ���")]
        public void AddPlayer_accountEvent_notManager()
        {
            Mock<IPlayerJoinRoom> IPlayerFactory = new Mock<IPlayerJoinRoom>();
            Mock<IInningeGame> IInningGameFactory = new Mock<IInningeGame>();
            var myRoom = new Room(DateTime.Now.AddHours(1), 10, IInningGameFactory.Object, IPlayerFactory.Object, ticketPrice_: 50);
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice, "")).Returns(true);
            player.Setup<decimal>(p => p.Account).Returns(0);
            var p2 = player.Object;
            p2.Id = 10;
            bool accountNotEnough = false;
            myRoom.PlayCanNotPayTicketEvent += delegate { accountNotEnough = true; };
            myRoom.AddPlayer(p2);
            Assert.False(accountNotEnough);
        }
        [Fact(DisplayName = "���������󴥷��¼�")]
        public void ChangeManager()
        {
            Mock<IPlayerJoinRoom> player = new Mock<IPlayerJoinRoom>();
            player.Setup<bool>(p => p.DecutMoney(_room.TicketPrice,"")).Returns(false);
            var p2 = player.Object;
            var preManager = _room.RoomManager;
            bool managerChanged = false;
            _room.ManagerChangedHandler += delegate { managerChanged = true; };
            _room.ChanageManger(p2);
            Assert.True(managerChanged);
        }
        [Fact(DisplayName = "�����Id��Ϊ��")]
        public void RoomInstall()
        {
            Assert.NotEmpty(_room.Id);
        }
    }

}


