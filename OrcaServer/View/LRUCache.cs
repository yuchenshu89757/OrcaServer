using System.Collections.Generic;
namespace OrcaServer.View
{
    public class LRUCache<K, V>
    {
        private int _size;//链表长度
        private readonly int _capacity;//缓存容量 
        private Dictionary<K, ListNode<K, V>> _dic;//key +缓存数据
        private ListNode<K, V> _linkHead;
        public LRUCache(int capacity)
        {
            _linkHead = new ListNode<K, V>(default(K), default(V));
            _linkHead.Next = _linkHead.Prev = _linkHead;
            _size = 0;
            _capacity = capacity;
            _dic = new Dictionary<K, ListNode<K, V>>();
        }

        public bool Contains(K key)
        {
            return _dic.ContainsKey(key);
        }

        public V Get(K key)
        {
            if (_dic.ContainsKey(key))
            {
                ListNode<K, V> n = _dic[key];
                MoveToHead(n);
                return n.Value;
            }
            return default(V);
        }
        public void Set(K key, V value)
        {
            ListNode<K, V> n;
            if (_dic.ContainsKey(key))
            {
                n = _dic[key];
                n.Value = value;
                MoveToHead(n);
            }
            else
            {
                n = new ListNode<K, V>(key, value);
                AttachToHead(n);
                _size++;
            }
            if (_size > _capacity)
            {
                RemoveLast();// 如果更新节点后超出容量，删除最后一个
                _size--;
            }
            _dic.Add(key, n);
        }
        // 移出链表最后一个节点
        private void RemoveLast()
        {
            ListNode<K, V> deNode = _linkHead.Prev;
            RemoveFromList(deNode);
            _dic.Remove(deNode.Key);
        }
        // 将一个孤立节点放到头部
        private void AttachToHead(ListNode<K, V> n)
        {
            n.Prev = _linkHead;
            n.Next = _linkHead.Next;
            _linkHead.Next.Prev = n;
            _linkHead.Next = n;
        }
        // 将一个链表中的节点放到头部
        private void MoveToHead(ListNode<K, V> n)
        {
            RemoveFromList(n);
            AttachToHead(n);
        }
        private void RemoveFromList(ListNode<K, V> n)
        {
            //将该节点从链表删除
            n.Prev.Next = n.Next;
            n.Next.Prev = n.Prev;
        }
    }

    public class ListNode<K, V>
    {
        public ListNode<K, V> Prev;
        public ListNode<K, V> Next;
        public K Key;
        public V Value;

        public ListNode(K key, V val)
        {
            Value = val;
            Key = key;
            Prev = null;
            Next = null;
        }
    }
}
