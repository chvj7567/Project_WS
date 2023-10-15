using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using DG.Tweening;

public enum PoolingScrollViewDirection
{
    Vertical,
    Horizenal,
}
public enum PoolingScrollViewAlign
{
    Left,
    Center,
    Right,
}

public enum PoolingScrollViewScrollingDirection
{
    Up,
    Down,
}

public class PoolingScrollViewItem<T>
{
    public int index;
    public T item;
}

[RequireComponent(typeof(ScrollRect))]
// ������Ʈ�� �ʿ��� ��ŭ �����ϰ�, ������ �ʴ� �� ������ �̷��.
// ������Ʈ�� ��Ȱ���ϰ� �ı����� �ʴ´�.
public abstract class CHPoolingScrollView<T, TInfo> : MonoBehaviour where T : MonoBehaviour
{
    protected LinkedList<PoolingScrollViewItem<T>> poolItems = new LinkedList<PoolingScrollViewItem<T>>();
    public List<T> Items
    {
        get
        {
            return poolItems.Select(x => x.item).ToList();
        }
    }

    protected List<TInfo> infos = new List<TInfo>();
    public List<TInfo> Infos
    {
        get
        {
            return infos.ToList();
        }
    }

    [Space]
    [ReadOnly]
    public PoolingScrollViewDirection direction = PoolingScrollViewDirection.Vertical;
    [Header("������ ũ��(GridLayout�� ��ũ��ó�� ����), zero�̸� origin ������� ����")]
    public Vector2 itemSize = Vector2.zero;
    [Header("�е� ����")]
    public RectOffset padding = new RectOffset();
    [Header("������ ���� ����")]
    public Vector2 itemGap = Vector2.zero;
    [Header("�� �࿡ ��ġ ����, 0�����̸�, ����Ʈ�ϰ� ���")]
    public int columnCount = 0;
    [Header("������ƮǮ ���� �����ϱ�, 0 �����̸�, ����Ʈ�ϰ� Ǯ ������ �Ҵ�")]
    public int poolItemCount = 0;
    [Header("������ ������ ������Ʈ")]
    public GameObject origin;
    [Header("������ ���� ����")]
    public PoolingScrollViewAlign align = PoolingScrollViewAlign.Center;
    [Header("��ũ�Ѻ������ �߰� ��, ������ ��ġ �����ϱ�")]
    public bool holdContentPositionWhenRefresh = false;
    [Header("��ũ�Ѻ� ������ ����Ǹ�, �ڵ����� ���ΰ�ħ")]
    public bool refreshWhenScrollViewResize = false;

    [Header("��ũ�Ѻ� ������ ����Ǹ�, ���̵��� ���ϸ��̼� ���")]
    public float fadeInDuration = 0;

    [Header("1�� �����Ӱ� ��ġ")]
    public bool addFreeLine = false;

    [Space]
    protected Vector2 prevScrollPosition = Vector2.zero;

    protected GameObject contentGameObject;
    protected RectTransform contentRectTransform;
    protected RectTransform viewPort;
    protected CanvasGroup contentCanvasGroup;

    protected ScrollRect scrollRect;

    protected int _columnCount = 0;

    protected bool originActive = false;

    public int LineCount
    {
        get
        {
            int line = 0;

            if (infos.Count != 0)
            {
                line = infos.Count / _columnCount;
                if (infos.Count % _columnCount != 0)
                {
                    line += 1;
                }
            }
            return line;
        }
    }

    public float LineLength
    {
        get
        {
            float len = _columnCount * itemSize.x;
            len += (_columnCount - 1) * itemGap.x;
            return len;
        }
    }

    // ��ũ�Ѻ� ���� ����ϱ�
    public float Height
    {
        get
        {
            int line = LineCount;
            float height = line * itemSize.y;
            height += (line - 1) * itemGap.y;
            height += padding.top + padding.bottom;
            return height;
        }
    }

    public virtual void Start()
    {
        if (direction == PoolingScrollViewDirection.Horizenal)
        {
            throw new NotSupportedException();
        }

        if (originActive == false)
        {
            origin.SetActive(false);
        }

        var scrollRect = GetComponent<ScrollRect>();
        scrollRect.OnValueChangedAsObservable().Subscribe(OnScroll);
        // ��ũ�Ѻ� ������ ��ȭ �����Ǹ�, �ٽ� �������ϱ�
        scrollRect.OnRectTransformDimensionsChangeAsObservable().Subscribe(_ =>
        {
            if (refreshWhenScrollViewResize)
            {
                Refresh();
            }

            if (fadeInDuration > 0 && contentCanvasGroup)
            {
                contentCanvasGroup.DOKill();
                contentCanvasGroup.alpha = 0;
                contentCanvasGroup.DOFade(1, fadeInDuration).SetEase(Ease.InSine);
            }
        });
    }

    public virtual void SetItemList(List<TInfo> list)
    {
        if (null == scrollRect) scrollRect = GetComponent<ScrollRect>();
        if (null == viewPort) viewPort = scrollRect.viewport;
        if (null == contentGameObject) contentGameObject = scrollRect.content.gameObject;
        if (null == contentRectTransform) contentRectTransform = scrollRect.content.GetComponent<RectTransform>();
        if (null == contentCanvasGroup) contentCanvasGroup = scrollRect.content.GetComponent<CanvasGroup>();

        infos.Clear();
        infos.AddRange(list);

        if (itemSize == Vector2.zero)
        {
            var rectTransform = origin.GetComponent<RectTransform>();
            itemSize.x = rectTransform.rect.width * rectTransform.localScale.x;
            itemSize.y = rectTransform.rect.height * rectTransform.localScale.y;
        }

        if (columnCount <= 0)
        {
            float width = viewPort.rect.width - (padding.left + padding.right);
            _columnCount = Mathf.FloorToInt(width / (itemSize.x + itemGap.x));
        }
        else
        {
            _columnCount = columnCount;
        }

        if (poolItemCount <= 0)
        {
            int line = Mathf.RoundToInt(viewPort.rect.size.y / itemSize.y);
            line += 2;

            if (addFreeLine)
            {
                line += 2;
            }

            poolItemCount = line * _columnCount;
        }

        InitContentTransform();
        CreatePoolingObject();
        InitContent();
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        if (poolItems.Count <= 0)
        {
            return;
        }

        Vector2 delta = scrollPosition - prevScrollPosition;
        prevScrollPosition = scrollPosition;
        if (delta.y != 0)
        {
            UpdateContent(delta.y > 0 ? PoolingScrollViewScrollingDirection.Up : PoolingScrollViewScrollingDirection.Down);
        }
    }

    private void UpdateContent(PoolingScrollViewScrollingDirection dir)
    {
        if (poolItems.Count <= 0)
        {
            return;
        }

        float scrollRectY = viewPort.rect.size.y;
        Rect contentRect = new Rect(0, -1 * (contentRectTransform.anchoredPosition.y + scrollRectY), contentRectTransform.rect.width, scrollRectY + itemSize.y);
        // �����˻��� ������ �簢���� ���Ʒ��� �����߰�
        Rect itemRect = new Rect();
        if (dir == PoolingScrollViewScrollingDirection.Up)
        {
            int firstIndex = poolItems.First.Value.index;
            for (int i = firstIndex - 1; i >= 0; --i)
            {
                Vector2 itemPosition = GetItemPosition(i);
                itemRect.Set(itemPosition.x, itemPosition.y, itemSize.x, itemSize.y);

                if (contentRect.Overlaps(itemRect))
                {
                    LinkedListNode<PoolingScrollViewItem<T>> node = poolItems.Last;
                    poolItems.Remove(node);

                    InitItem(node.Value.item, i);

                    node.Value.index = i;
                    poolItems.AddFirst(node);
                }
            }
        }
        else if (dir == PoolingScrollViewScrollingDirection.Down)
        {
            int lastIndex = poolItems.Last.Value.index;
            for (int i = lastIndex + 1; i < infos.Count; ++i)
            {
                Vector2 itemPosition = GetItemPosition(i);
                itemRect.Set(itemPosition.x, itemPosition.y, itemSize.x, itemSize.y);

                if (contentRect.Overlaps(itemRect))
                {
                    var node = poolItems.First;
                    poolItems.Remove(node);

                    InitItem(node.Value.item, i);

                    node.Value.index = i;
                    poolItems.AddLast(node);
                }
            }
        }
    }

    private void InitContent()
    {
        poolItems.Clear();
        int childCount = contentGameObject.transform.childCount;
        GameObject[] children = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = contentGameObject.transform.GetChild(i).gameObject;
        }

        float scrollRectY = viewPort.rect.size.y;
        Rect contentRect = new Rect(0, -1 * (contentRectTransform.anchoredPosition.y + scrollRectY), contentRectTransform.rect.width, scrollRectY + itemSize.y);
        Rect itemRect = new Rect();
        int firstIndex = Enumerable.Range(0, infos.Count).FirstOrDefault(i =>
        {
            Vector2 itemPosition = GetItemPosition(i);
            itemRect.Set(itemPosition.x, itemPosition.y, itemSize.x, itemSize.y);
            return contentRect.Overlaps(itemRect);
        });

        for (int i = 0; i < childCount; i++)
        {
            children[i].SetActive(true);
        }

        for (int i = 0; i < children.Length; ++i)
        {
            int index = i + firstIndex;
            T item = children[i].GetComponent<T>();
            InitItem(item, index);
            PoolingScrollViewItem<T> poolItem = new PoolingScrollViewItem<T>() { index = index, item = item };
            poolItems.AddLast(poolItem);
        }
    }

    private void InitItem(T item, int index)
    {
        TInfo info = infos.ElementAtOrDefault(index);
        item.gameObject.SetActive(info != null);
        if (info != null)
        {
            InitItem(item, info, index);
        }
        InitItemTransform(item.gameObject, index);
        item.gameObject.name = $"{origin.name} {index}";
    }

    public abstract void InitItem(T obj, TInfo info, int index);
    public virtual void InitPoolingObject(T obj)
    {
    }

    public virtual void InitContentTransform()
    {
        // ��Ʈ��ġ ��Ŀ�� ����
        contentRectTransform.anchorMax = new Vector2(0.5f, 1);
        contentRectTransform.anchorMin = new Vector2(0.5f, 1);
        contentRectTransform.pivot = new Vector2(0.5f, 1);

        // ������ �缳��
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewPort.rect.width);
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
        if (holdContentPositionWhenRefresh) // ����Ʈ�� ��ũ�Ѻ亸�� �Ʒ��� ���� ��, ��ũ�Ѻ� �ϴ����� ��ǥ �Ű��ֱ�
        {
            float maxY = Height - viewPort.rect.height;
            float y = Mathf.Min(maxY, contentRectTransform.anchoredPosition.y);
            contentRectTransform.anchoredPosition = new Vector2(0, y);
        }
        else
        {
            // ����Ʈ �ֻ������ �̵�
            contentRectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public virtual void InitItemTransform(GameObject item, int index)
    {
        var rectTransform = item.GetComponent<RectTransform>();

        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);

        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x);
        //rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y);

        rectTransform.anchoredPosition = GetItemPosition(index);
        rectTransform.SetSiblingIndex(index);
    }

    public virtual Vector2 GetItemPosition(int index)
    {
        float width = contentRectTransform.rect.width;
        int colIndex = index % _columnCount;
        float x = colIndex * itemSize.x;
        x += colIndex * itemGap.x;
        float diff = width - LineLength;
        if (diff > 0)
        {
            switch (align)
            {
                case PoolingScrollViewAlign.Left:
                    x += 0;
                    break;
                case PoolingScrollViewAlign.Center:
                    x += (width - LineLength) * 0.5f;
                    break;
                case PoolingScrollViewAlign.Right:
                    x += width - LineLength;
                    break;
            }
        }
        if (diff > (padding.left + padding.right))
        {
            switch (align)
            {
                case PoolingScrollViewAlign.Left:
                    x += padding.left;
                    break;
                case PoolingScrollViewAlign.Center:
                    x += padding.left - padding.right;
                    break;
                case PoolingScrollViewAlign.Right:
                    x += -1 * padding.right;
                    break;
            }
        }

        int rowIndex = index / _columnCount;
        float y = rowIndex * itemSize.y;
        y += rowIndex * itemGap.y;
        y += padding.top;
        y *= -1;

        return new Vector2(x, y);
    }

    private void CreatePoolingObject()
    {
        int diff = poolItemCount - contentGameObject.transform.childCount;
        diff = Mathf.Min(diff, infos.Count); // �ʿ��� ��ŭ�� Ǯ�� ������
        if (diff > 0)
        {
            for (int i = 0; i < diff; ++i)
            {
                var obj = Instantiate(origin, contentGameObject.transform);
                InitPoolingObject(obj.GetComponent<T>());
            }
        }
    }

    public void RefreshWithColumnCount(int column = 0)
    {
        columnCount = column;
        Refresh();
    }

    public void Refresh()
    {
        SetItemList(infos.ToList());
    }

    public void Clear()
    {
        infos.Clear();
        poolItems.Clear();
        if (contentGameObject)
        {
            int childCount = contentGameObject.transform.childCount;
            GameObject[] children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = contentGameObject.transform.GetChild(i).gameObject;
            }

            foreach (var child in children)
            {
                child.SetActive(false);
            }
        }
    }
}
