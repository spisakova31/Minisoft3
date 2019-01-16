using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropBranch : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Game game;
    bool dragging;
    Vector3 originPosition;
    RectTransform thisRT;
    int position = -1;
    bool placed = false;
    int indexThis;
    float width;
    float height;
    RectTransform helpBranchRect;

    private void Start()
    {
        thisRT = (RectTransform)transform;
        dragging = false;
        var canvas = GameObject.FindWithTag("Canvas");
        game = canvas.GetComponent<Game>();
        SetPosition(gameObject, transform.position);

    }

    public void SetPosition(GameObject gameObject, Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (game.GetLevel() > 6)
        {
            return;
        }
        helpBranchRect = game.GetHelpBranchRect();
        width = helpBranchRect.rect.width;
        height = helpBranchRect.rect.height;
        originPosition = gameObject.transform.position; 
        
        dragging = true;
        transform.SetAsLastSibling();
        game.crossbarImg.transform.SetAsLastSibling();
        indexThis = (int)((gameObject.transform.position.x - helpBranchRect.transform.position.x) / (width + 10));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (game.GetLevel() > 6)
        {
            return;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (game.GetLevel() > 6)
        {
            return;
        }
        int originIndex = (int)((originPosition.x - helpBranchRect.transform.position.x) / (width + 10));
        for (var i = 0; i < game.GetAllBranches().Count; i++)
        {
            GameObject branch = game.GetAllBranches()[i];
            
            if (branch != gameObject)
            {
                
                if (branch.transform.position.x - width / 2 < gameObject.transform.position.x && branch.transform.position.x + width / 2 > gameObject.transform.position.x
                    && branch.transform.position.y - height / 2 < gameObject.transform.position.y && branch.transform.position.y + height / 2 > gameObject.transform.position.y)
                {               
                    int index = (int)((branch.transform.position.x - helpBranchRect.transform.position.x) / (width + 10));
                    if ((indexThis - index == 1) || (indexThis - index == -1))
                    {
                        gameObject.transform.position = new Vector3(
                            helpBranchRect.transform.position.x + index * (width + 10),
                            helpBranchRect.transform.position.y,
                            0);
                        branch.transform.position = new Vector3(
                            helpBranchRect.transform.position.x + indexThis * (width + 10),
                            helpBranchRect.transform.position.y,
                            0);
                        game.IncreaseMoves();
                        game.SwitchIndexes(index, indexThis);
                        return;
                    }
                    else
                    {
                        transform.position = originPosition;
                    }

                }
            }
            dragging = false;
        }
        transform.position =  originPosition;

    }
}