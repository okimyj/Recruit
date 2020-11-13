using System.Collections.Generic;
using UnityEngine;
public class UINavigationStack : AbstractSingleton<UINavigationStack>
{
	private Dictionary<string, UIBase> createdUIMap = new Dictionary<string, UIBase>();
	private List<UIBase> shownUIList = new List<UIBase>();
	private Vector3 lastUIPosition = new Vector3(1000, 1000, 0);
	private Vector3 addPosition = new Vector3(100, 0, 0);
	// ui 생성, 한번 생성한 ui는 hide될 때 object를 없애는게 아니라 createMap에 넣어뒀다가 재사용한다.
	public UIBase CreateUI(GameObject prefab)
	{
		if(createdUIMap.ContainsKey(prefab.name))
		{
			return createdUIMap[prefab.name];
		}
		else
		{
			GameObject ui = Instantiate(prefab) as GameObject;
			ui.transform.localPosition = lastUIPosition;
			lastUIPosition += addPosition;
			UIBase uiBase = ui.GetComponent<UIBase>();
			createdUIMap[prefab.name] = uiBase;
			return uiBase;
		}
	}
	public UIBase GetTopUIInStack()
	{
		// 가장 상단에 있는 UI를 return.
		UIBase topUI = null;
		if (null != shownUIList && shownUIList.Count > 0)
		{
			int index = shownUIList.Count;
			topUI = shownUIList[--index];
		}
		return topUI;
	}
	public void Push(UIBase ui)
	{
		UIBase topUI = GetTopUIInStack();
		int sortOrderBase = 0;
		if (null != topUI && null != topUI.canvases && topUI.canvases.Length > 0)
			sortOrderBase = topUI.canvases[topUI.canvases.Length - 1].sortingOrder;
		shownUIList.Add(ui);
		// canvas의 sortingOrder 변경.
		// push 되는 순서에 맞게 변경해주지 않으면 위에 있는 ui가 Pop되는 경우 ui가 제대로 나타나지 않을 수 있다. 
		if (null != ui.canvases && ui.canvases.Length > 0)
		{
			for (int i=0; i < ui.canvases.Length; ++i)
			{
				sortOrderBase = sortOrderBase + 1;
				ui.canvases[i].sortingOrder = sortOrderBase;
			}
		}
		UIShow(ui, true);
	}
	public void Pop(UIBase ui)
	{
		shownUIList.Remove(ui);
		UIShow(ui, false);
	}
	public void UIShow(UIBase ui, bool show)
	{
		ui.gameObject.SetActive(show);
	}
	public void BackPressed()
	{
		if (null != GetTopUIInStack())
		{
			// 마지막에 1개 ui가 남아있을때는 없애지 않도록 한다. (mainUI)
			if (null != shownUIList && shownUIList.Count > 1)
			{
				GetTopUIInStack().BackPressed();
				return;
			}
		}		
	}
	void LateUpdate()
	{
		// 뒤로가기 버튼이 눌렸을 때.
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			BackPressed();
		}
	}
}

