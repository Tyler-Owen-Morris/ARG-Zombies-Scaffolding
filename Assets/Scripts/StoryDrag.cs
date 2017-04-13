using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class StoryDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	private Vector2 startPosition;
    private WeaponSelectManager myLevelManager;
	//private Vector2 offset;

	void Start () {
        //store beginning location
        myLevelManager = GameManager.FindObjectOfType<WeaponSelectManager>();
		startPosition = this.gameObject.transform.position;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		//store the offset
		//offset = startPosition + eventData.position;
	}

	public void OnDrag(PointerEventData eventData) {
		

		Vector2 eventPosition = eventData.position;
		if (eventData.position.x > startPosition.x) {
			eventPosition.x = startPosition.x;
		}
		eventPosition.y=startPosition.y;

		this.transform.position = eventPosition;
	}

	public void OnEndDrag(PointerEventData eventData) {
		float dragDist = 3.0f;
		if (transform.position.x <= (startPosition.x-dragDist)) {
            myLevelManager.PanelDestroyed();
            Destroy(this.gameObject);
		} 
	}

	
}
