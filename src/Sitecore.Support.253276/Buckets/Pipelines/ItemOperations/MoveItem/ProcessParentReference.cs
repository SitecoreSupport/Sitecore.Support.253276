using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;

namespace Sitecore.Support.Buckets.Pipelines.ItemOperations.MoveItem
{
  public class ProcessParentReference : Sitecore.Buckets.Pipelines.ItemOperations.MoveItem.ProcessParentReference
  {
    private static readonly ID ParentReferenceFieldId = ID.Parse(Sitecore.Buckets.Util.Constants.BucketParentReference);

    public new void Process(PipelineArgs args)
    {
      if ((args != null) && this.MeetProcessConditions(args))
      {
        Item source = this.GetSource(args);
        if (this.IsBucket(source) || this.IsItemContainedWithinBucket(source))
        {
          this.ProcessParentReferenceRecursive(this.GetItemToProcess(args));
        }
      }
    }

    private void ProcessParentReferenceRecursive(Item itemToProcess)
    {
      using (new SecurityDisabler())
      {
        if (itemToProcess.Versions.Count > 0)
        {
          using (new EditContext(itemToProcess))
          {
            itemToProcess[ParentReferenceFieldId] = string.Empty;
          }
        }
        else
        {
          using (new EditContext(itemToProcess, false, false))
          {
            itemToProcess[ParentReferenceFieldId] = string.Empty;
          }
        }
      }
      foreach (Item item in itemToProcess.GetChildren(ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity))
      {
        this.ProcessParentReferenceRecursive(item);
      }
    }
  }
}